using System;
using PubSubWeb.Tool;
using ServiceStack.Redis;

namespace PubSubWeb.Redis
{

    public abstract class SingleClass<T> where T : new()
    {
        public static readonly T Instance = new Lazy<T>(() => new T()).Value;
    }

    public class RedisService : SingleClass<RedisService>
    {
        private readonly PooledRedisClientManager _redis;

        public PooledRedisClientManager pooledRedisClient => _redis;

        public IRedisClient Client => _redis.GetClient();

        public RedisService()
        {
            var readWriteHosts = new[] { AppSettingsHelper.GetString("Redis:RedisAddr") };
            var readOnlyHosts = new[] { AppSettingsHelper.GetString("Redis:RedisAddr") };
            var initialDb = AppSettingsHelper.GetInt32("Redis:RedisDb");
            _redis = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig()
            {
                MaxWritePoolSize = 5,
                MaxReadPoolSize = 5,
                AutoStart = true
            }, initialDb, 50, 5);
        }
    }

}
