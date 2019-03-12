using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ChatWeb.Model;

namespace ChatWeb.Tool
{
    /// <summary>
    /// 队列处理
    /// <para>适合多个生产者一个消费者的情景</para>
    /// </summary>
    public class AsyncQueue<T>
    {
        #region 字段、属性

        //有线程正在处理数据
        private const int Processing = 1;
        //没有线程处理数据
        private const int UnProcessing = 0;
        //队列是否正在处理数据
        private int _isProcessing;
        //队列是否可用
        private volatile bool _enabled = true;
        //当前任务
        private Task _currentTask;
        //队列
        private readonly ConcurrentQueue<T> _queue;

        #endregion

        #region 事件

        public event Action<T> ProcessItemFunction;
        public event EventHandler<EventArgs> ProcessException;

        #endregion

        #region 构造

        //public static readonly AsyncQueue<T> Instance = new Lazy<AsyncQueue<T>>(() => new AsyncQueue<T>()).Value;

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="isRunBackgroundTask">开始一个线程防止遗漏</param>
        public AsyncQueue(bool isRunBackgroundTask = false)
        {
            _queue = new ConcurrentQueue<T>();
            if (isRunBackgroundTask)
            {
                Start();
            }
        }

        #endregion

        #region 操作

        /// <summary>
        /// 开始
        /// </summary>
        private void Start()
        {
            var processThread = new Thread(PorcessItem) {IsBackground = true};
            processThread.Start();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Stop()
        {
            _enabled = false;
        }

        /// <summary>
        /// 消息入队
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (item == null) return;
            _queue.Enqueue(item);
            DataAdded();
        }

        /// <summary>
        /// 数据添加完成后通知消费者线程处理
        /// </summary>
        private void DataAdded()
        {
            if (_enabled)
            {
                if (!IsProcessingItem())
                {
                    _currentTask = Task.Factory.StartNew(ProcessItemLoop);
                }
            }
        }

        /// <summary>
        /// 判断是否队列有线程正在处理 
        /// </summary>
        /// <returns></returns>
        private bool IsProcessingItem()
        {
            return Interlocked.CompareExchange(ref _isProcessing, Processing, UnProcessing) != 0;
        }

        /// <summary>
        /// 循环出队
        /// </summary>
        private void ProcessItemLoop()
        {
            if (!_enabled && _queue.IsEmpty)
            {
                Interlocked.Exchange(ref _isProcessing, 0);
                return;
            }
            if (_queue.TryDequeue(out var publishFrame))
            {
                try
                {
                    ProcessItemFunction(publishFrame);
                }
                catch (Exception ex)
                {
                    OnProcessException(ex);
                }
            }
            if (_enabled && !_queue.IsEmpty)
            {
                _currentTask = Task.Factory.StartNew(ProcessItemLoop);
            }
            else
            {
                Interlocked.Exchange(ref _isProcessing, UnProcessing);
            }
        }

        /// <summary>
        ///定时处理线程调用函数  
        ///主要是监视入队的时候线程 没有来的及处理的情况
        /// </summary>
        private void PorcessItem(object state)
        {
            var sleepCount = 0;
            var sleepTime = 1000;
            while (_enabled)
            {
                //如果队列为空则根据循环的次数确定睡眠的时间
                if (_queue.IsEmpty)
                {
                    if (sleepCount == 0)
                    {
                        sleepTime = 1000;
                        sleepCount++;
                    }
                    else if (sleepCount <= 3)
                    {
                        sleepTime = 1000 * 3;
                        sleepCount++;
                    }
                    else
                    {
                        sleepTime = 1000 * 60 * 5;
                    }
                    Thread.Sleep(sleepTime);
                }
                else
                {
                    //判断是否队列有线程正在处理 
                    if (_enabled && !IsProcessingItem())
                    {
                        if (!_queue.IsEmpty)
                        {
                            _currentTask = Task.Factory.StartNew(ProcessItemLoop);
                        }
                        else
                        {
                            Interlocked.Exchange(ref _isProcessing, 0);
                        }
                        sleepCount = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 完成任务后退出
        /// </summary>
        public void Finish()
        {
            Stop();

            _currentTask?.Wait();

            while (!_queue.IsEmpty)
            {
                try
                {
                    if (_queue.TryDequeue(out var publishFrame))
                    {
                        ProcessItemFunction(publishFrame);
                    }
                }
                catch (Exception ex)
                {
                    OnProcessException(ex);
                }
            }
            _currentTask = null;
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="ex"></param>
        private void OnProcessException(Exception ex)
        {
            var tempException = ProcessException;
            Interlocked.CompareExchange(ref ProcessException, null, null);

            if (tempException != null)
            {
                ProcessException(ex, new EventArgs<Exception>(ex));
            }
        }
        #endregion

    }
}
