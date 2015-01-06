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

                //TODO: the following line adds a new user to the database
                //      you may with to change how this works for your program.
                database.Users[username] = new User() { UserName = username };

                if (!database.TrySave(path)) ExitWithError("Could not save database.");
            }
            else //Authenticate mode
            {
                //TODO: Add some form of authentication to determine if correct
                //      credentials were presented.
                if (!database.Users.ContainsKey(username)) ExitWithError("Invalid user");
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
