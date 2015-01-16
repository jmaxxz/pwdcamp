using System;
using ConsoleOptions;
using BCrypt.Net;

namespace authenticate
{
    class Program
    {
        static void Main(string[] args)
        {
            bool addMode = false;
            bool newDbMode = false;

            string username = "";
            string password = "";
            string path = "database.json";
            var hasher = new BCrypt.Net.BCrypt();


            var options = new Options("Creates and manages a simple database of users.")
            {
                new Option(new[]{"a","add"}, ()=>addMode=true, "Adds a new user"),
                new Option((s)=>username=s.ToLowerInvariant(), "username", "The username to be used"),
                new Option((s)=>password=s, "password", "The password of the user"),
                new Option(new[]{"i","init"}, ()=>newDbMode=true, "Does not attempt to load existing database")
            };

            if (!options.Parse(args)) ExitWithError("Invalid options");

            Database database = null;
            database = !newDbMode && Database.TryLoad(path, out database) ? database : new Database();

            if(addMode)
            {
                if(database.Users.ContainsKey(username)) ExitWithError("User Exists");

                var salt = BCrypt.Net.BCrypt.GenerateSalt(4);
                var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

                database.Users[username] = new User() { UserName = username, Hash = hash };

                if (!database.TrySave(path)) ExitWithError("Could not save database.");
            }
            else //Authenticate mode
            {

                if (!database.Users.ContainsKey(username))
                {
                    ExitWithError("Authentication failed.");
                }
                else
                {
                    var existingHash = database.Users[username].Hash;
                    var verified = BCrypt.Net.BCrypt.Verify(password, existingHash);
                    if (!verified) ExitWithError("Authentication failed");
                }
            }

            Console.WriteLine("Success");
        }

        private static void ExitWithError(string p)
        {
            Console.Error.WriteLine("ERROR: " + p);
            Environment.Exit(1);
        }
    }
}
