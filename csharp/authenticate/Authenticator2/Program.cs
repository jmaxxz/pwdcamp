using System;
using System.Security.Cryptography;
using ConsoleOptions;

namespace Authenticator2
{
    static class Program
    {
        private static string ComputeHash(string username, string password)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            // use 'username' as a basic salt - not a good method in general, but ok for this example; at least it's guaranteed to be unique
            byte[] data = System.Text.Encoding.UTF8.GetBytes(username + password);
            data = algorithm.ComputeHash(data);
            return Convert.ToBase64String(data);
        }

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
                new Option(s=>username=s.ToLowerInvariant(), "username", "The username to be used"),
                new Option(s=>password=s, "password", "The password of the user"),
                new Option(new[]{"i","init"}, ()=>newDbMode=true, "Does not attempt to load existing database")
            };

            if (!options.Parse(args)) ExitWithError("Invalid options");

            Database database = !newDbMode && Database.TryLoad(path, out database) ? database : new Database();

            if(addMode)
            {
                if(database.Users.ContainsKey(username)) ExitWithError("User Exists");

                // add the new user to the database
                database.Users[username] = new User { UserName = username, PassHash = ComputeHash(username, password)};

                if (!database.TrySave(path)) ExitWithError("Could not save database.");
            }
            else //Authenticate mode
            {
                // Determine if correct credentials were presented.
                if (!database.Users.ContainsKey(username) || database.Users[username].PassHash != ComputeHash(username, password)) ExitWithError("Invalid user");
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
