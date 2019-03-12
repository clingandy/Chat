using System;
using ChatWeb.Tool;
using StackExchange.Redis;

namespace ChatWeb.Redis
{

    public abstract class SingleClass<T> where T : new()
    {
        public static readonly T Instance = new Lazy<T>(() => new T()).Value;
    }

    public class RedisService : SingleClass<RedisService>
    {
        public IDatabase RedisDb { get; }

        public IConnectionMultiplexer Proxy { get; }

        public RedisService()
        {
            Proxy = ConnectionMultiplexer.Connect(AppSettingsHelper.GetString("Redis:RedisAddr"));
            RedisDb = Proxy.GetDatabase(AppSettingsHelper.GetInt32("Redis:RedisDb"));

            //事件注册
            Proxy.ConnectionFailed += MuxerConnectionFailed;
            Proxy.ConnectionRestored += MuxerConnectionRestored;
            Proxy.ErrorMessage += MuxerErrorMessage;
            Proxy.ConfigurationChanged += MuxerConfigurationChanged;
            Proxy.HashSlotMoved += MuxerHashSlotMoved;
            Proxy.InternalError += MuxerInternalError;
        }

        /// <summary>
        /// 内部异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
        }

        /// <summary>
        /// 集群更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
        }

        /// <summary>
        /// 配置更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
        }

        /// <summary>
        /// 错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
        }

        /// <summary>
        /// 重连错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
        }

        /// <summary>
        /// 连接失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
        }
    }

}
