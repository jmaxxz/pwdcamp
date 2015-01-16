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

                var salter = new RNGCryptoServiceProvider();
                var saltBytes = new byte[12];
                salter.GetBytes(saltBytes);
                var saltString = Convert.ToBase64String(saltBytes);
                var hasher = SHA1.Create();
                var hashedPwBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(saltString + password));
                var hashedPwString = Convert.ToBase64String(hashedPwBytes);
                database.Users[username] = new User() { UserName = username, Password = hashedPwString, Salt = saltString };

                if (!database.TrySave(path)) ExitWithError("Could not save database.");
            }
            else //Authenticate mode
            {
                if (!database.Users.ContainsKey(username)) ExitWithError("Invalid user");
                var user = database.Users[username];
                var authenticated = user.Password.Equals(Hash(user.Salt + password));
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
