using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
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
            string path = "database.json";


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

                var hasher = SHA1.Create();
                var hashedPwBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPwString = Convert.ToBase64String(hashedPwBytes);
                database.Users[username] = new User() { UserName = username, Password = hashedPwString };

                if (!database.TrySave(path)) ExitWithError("Could not save database.");
            }
            else //Authenticate mode
            {
                if (!database.Users.ContainsKey(username)) ExitWithError("Invalid user");
                var authenticated = database.Users[username].Password.Equals(Hash(password));
                Console.WriteLine(authenticated ? "Authenticated" : "Auth fail");
            }

            Console.WriteLine("Success");
        }

        private static void ExitWithError(string p)
        {
            Console.Error.WriteLine("ERROR: " + p);
            Environment.Exit(1);
        }

        private static string Hash(string s)
        {
            var hasher = SHA1.Create();
            var hashedPwBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToBase64String(hashedPwBytes);
        }
    }
}
