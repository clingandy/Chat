using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RedisAccessor.Tracing;
using StackExchange.Redis;

namespace RedisAccessor
{
    public class RedisConnection : IRedisConnection
    {
        #region Field

        private ISubscriber _redisSubscriber;
        private TraceSource _trace;

        private readonly object _shutdownLock = new object();
        private bool _disposed = false;

        #endregion

        #region Property

        public IDatabase RedisDb { get; private set; }
        public ConnectionMultiplexer Proxy { get; private set; }

        #endregion

        #region Method

        public async Task ConnectAsync(string connectionString, int db, TraceSource trace)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RedisConnection));
            }
            var connection = await ConnectionMultiplexer.ConnectAsync(connectionString, new TraceTextWriter("ConnectionMultiplexer: ", trace));
            lock (_shutdownLock)
            {
                if (_disposed)
                {
                    _trace.TraceVerbose("Connection closed during connect");
                    connection.Dispose();
                    return;
                }
                Proxy = connection;
                if (!Proxy.IsConnected)
                {
                    Proxy.Dispose();
                    Proxy = null;
                    throw new InvalidOperationException("Failed to connect to Redis");
                }

                Proxy.ConnectionFailed += OnConnectionFailed;
                Proxy.ConnectionRestored += OnConnectionRestored;
                Proxy.ErrorMessage += OnError;

                _trace = trace;
                RedisDb = Proxy.GetDatabase(db);
                _redisSubscriber = Proxy.GetSubscriber();
            }
        }

        public void Close(bool allowCommandsToComplete = true)
        {
            lock (_shutdownLock)
            {
                if (_disposed)
                {
                    return;
                }

                _trace.TraceInformation("Closing all key");
                if (_redisSubscriber != null)
                {
                    _redisSubscriber.UnsubscribeAll();
                }

                if (Proxy != null)
                {
                    Proxy.Close(allowCommandsToComplete);
                    Proxy.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            lock (_shutdownLock)
            {
                if (_disposed)
                {
                    return;
                }

                if (Proxy != null)
                {
                    _trace.TraceVerbose("Disposing connection");
                    Proxy.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion

        #region Event

        public event Action<Exception> ConnectionFailed;
        public event Action<Exception> ConnectionRestored;
        public event Action<Exception> ErrorMessage;

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs args)
        {
            _trace.TraceWarning($"{args.ConnectionType} Connection failed. Reason: {args.FailureType} Exception: {args.Exception}");
            var handler = ConnectionFailed;
            handler(args.Exception);
        }

        private void OnConnectionRestored(object sender, ConnectionFailedEventArgs args)
        {
            if (_trace.Switch.ShouldTrace(TraceEventType.Information))
            {
                _trace.TraceInformation($"{args.ConnectionType} Connection failed. Reason: {args.FailureType} Exception: {args.Exception?.ToString() ?? "<none>"}");
            }
            var handler = ConnectionRestored;
            handler(args.Exception);
        }

        private void OnError(object sender, RedisErrorEventArgs args)
        {
            _trace.TraceWarning($"Redis Error: {args.Message}");
            var handler = ErrorMessage;
            handler(new InvalidOperationException(args.Message));
        }

        #endregion
    }
}
