using System;

namespace RedisAccessor
{
    public static class DependencyExtensions
    {
        public static RedisHelper UseRedis(string server, int port, string password, int db, string prefixKey)
        {
            var configuration = new RedisConfiguration(server, port, password, db, prefixKey);

            return RedisHelper(configuration);
        }

        public static RedisHelper UseRedis(string serverAddr, int db, string prefixKey)
        {
            var configuration = new RedisConfiguration(serverAddr, db, prefixKey);

            return RedisHelper(configuration);
        }

        private static RedisHelper RedisHelper(RedisConfiguration configuration)
        {
            return new Lazy<RedisHelper>(() => new RedisHelper(configuration, new RedisConnection())).Value;
        }

       
    }
}
