using System;
using System.IO;
using Newtonsoft.Json;

namespace LobuliMeasurement
{
    public static class Config
    {
        private static bool _loadet = false;
        
        public static string MySQLServer = "";
        public static string MySQLDatabase = "";
        public static string MySQLUser = "";
        public static string MySQLPass = "";
        public static string HomepageBaseUrl = "";

        private static string ConfigFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mysql.config");

        public static bool IsLoadet()
        {
            return _loadet;
        }

        public static void Load()
        {
            using (StreamReader sr = new StreamReader(ConfigFilePath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                MySqlConfig config = serializer.Deserialize<MySqlConfig>(reader);

                MySQLServer = config.Server;
                MySQLDatabase = config.Database;
                MySQLUser = config.User;
                MySQLPass = config.Password;
                HomepageBaseUrl = config.HomepageBaseUrl;

                _loadet = true;

            }
        }

        public static bool ConfigExist()
        {
            return File.Exists(ConfigFilePath);
        }
        
        public static void Safe(string server, string database, string user, string pass, string homepageBaseUrl)
        {
            using (StreamWriter file = File.CreateText(ConfigFilePath))
            using (JsonWriter writer = new JsonTextWriter(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, new MySqlConfig(server, database, user, pass, homepageBaseUrl));
            }
        }

    }

    public class MySqlConfig
    {
        public string Server;
        public string Database;
        public string User;
        public string Password;
        public string HomepageBaseUrl;

        public MySqlConfig(string server, string database, string user, string pass, string homepageBaseUrl)
        {
            Server = server;
            Database = database;
            User = user;
            Password = pass;
            HomepageBaseUrl = homepageBaseUrl;
        }
    }
}
