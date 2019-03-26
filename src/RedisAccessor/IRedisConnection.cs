using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisAccessor
{
    public interface IRedisConnection
    {
        IDatabase RedisDb { get;}

        ConnectionMultiplexer Proxy { get;}

        Task ConnectAsync(string connectionString, int db, TraceSource trace);

        void Close(bool allowCommandsToComplete = true);

        void Dispose();

        event Action<Exception> ConnectionFailed;
        event Action<Exception> ConnectionRestored;
        event Action<Exception> ErrorMessage;
    }
}
