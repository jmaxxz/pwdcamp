using System;
using ConsoleOptions;

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
            const string path = "database.json";


            var options = new Options("Creates and manages a simple database of users.")
            {
                new Option(new[]{"a","add"}, ()=>addMode=true, "Adds a new user"),
                new Option((s)=>username=s.ToLowerInvariant(), "username", "The username to be used"),
                new Option((s)=>password=s, "password", "The password of the user"),
                new Option(new[]{"i","init"}, ()=>newDbMode=true, "Does not attempt to load existing database")
            };

            if (!options.Parse(args)) ExitWithError("Invalid options");

            Database database;
            database = !newDbMode && Database.TryLoad(path, out database) ? database : new Database();

            if(addMode)
            {
                if(database.Users.ContainsKey(username)) ExitWithError("User Exists");

                var crypticPw = BCrypt.Net.BCrypt.HashPassword(password);
                database.Users[username] = new User() { UserName = username, Password = crypticPw };

                if (!database.TrySave(path)) ExitWithError("Could not save database.");
            }
            else //Authenticate mode
            {
                User user;
                if (database.Users.TryGetValue(username, out user))
                {
                    var authenticated = BCrypt.Net.BCrypt.Verify(password, user.Password);
                    Console.WriteLine(authenticated ? "Authenticated" : "Not Authenticated");
                }
                else ExitWithError("Invalid user");
            }

            Console.WriteLine("Program Complete");
        }

        private static void ExitWithError(string p)
        {
            Console.Error.WriteLine("ERROR: " + p);
            Environment.Exit(1);
        }
    }
}
