using System;
using System.Globalization;

namespace RedisAccessor
{
    public class RedisConfiguration
    {

        public RedisConfiguration(string server, int port, string password, int db, string prefixKey)
            : this(CreateConnectionString(server, port, password), db, prefixKey)
        {
        }

        private static string CreateConnectionString(string server, int port, string password)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}:{1}, password={2}, abortConnect=false", server, port, password);
        }

        public RedisConfiguration(string connectionString, int db, string prefixKey)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            Db = db;
            PrefixKey = prefixKey;
            ConnectionString = connectionString;
            
            //if (connectionString.Length > 0)
            //{
            //    var options = ConfigurationOptions.Parse(connectionString);
            //    Db = options.DefaultDatabase.GetValueOrDefault(db);
            //}
        }

        internal string ConnectionString { get; }

        public int Db { get; set; }

        public string PrefixKey { get; set; }


    }
}
