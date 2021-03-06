﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace authenticate
{
    public class Database
    {
        public IDictionary<string, User> Users {get; private set;}

        public Database()
        {
            Users = new Dictionary<string, User>();
        }

        public bool TrySave(string path)
        {
            var rawDb = JsonConvert.SerializeObject(this);
            try
            {
                File.WriteAllText(path, rawDb, Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool TryLoad(string path, out Database database)
        {
            try
            {
                var rawDb = File.ReadAllText(path, Encoding.UTF8);
                database = JsonConvert.DeserializeObject<Database>(rawDb);
            }
            catch
            {
                database = default(Database);
                return false;
            }

            return true;
        }
    }
}
