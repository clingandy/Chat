using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ChatWeb.Redis
{
    public class RedisHelper
    {
        #region 属性、构造

        private static readonly string _channelListKey = "pubsub_channels";

        private IDatabase _redisDb => RedisService.Instance.RedisDb;

        public static readonly RedisHelper Instance = new Lazy<RedisHelper>(() => new RedisHelper()).Value;

        #endregion

        #region 取消、订阅渠道

        /// <summary>
        /// 订阅渠道 
        /// </summary>
        public void OnSubscribe(string channelName, Action<string> sendMsg)
        {
            var sub = RedisService.Instance.Proxy.GetSubscriber();
            sub.Subscribe(channelName, (channel, value) =>
            {
                sendMsg.Invoke(value.ToString());
            });

            //using (var consumer = RedisService.Instance.Client)
            //{
            //    //创建订阅
            //    var subscription = consumer.CreateSubscription();

            //    //接收消息处理Action
            //    subscription.OnMessage = (channel, msg) =>
            //    {
            //        sendMsg.Invoke(msg);
            //    };

            //    //订阅事件处理Action
            //    subscription.OnSubscribe = channel =>
            //    {
                    
            //    };

            //    //取消订阅事件处理Action
            //    subscription.OnUnSubscribe = channel =>
            //    {
            //        OnSubscribe(channelName, sendMsg);
            //    };

            //    //订阅渠道
            //    subscription.SubscribeToChannels(channelName);
            //}
        }

        /// <summary>
        /// 取消订阅渠道
        /// </summary>
        public void OnUnSubscribe(string channelName)
        {
           RedisService.Instance.Proxy.GetSubscriber().Unsubscribe(channelName);
        }

        #endregion

        #region 消息管理

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMsg(string channel, string msg)
        {
            Task.Factory.StartNew(() =>
            {
                _redisDb.Publish(channel, msg);
                // 保存消息
                // SaveMsg(channel, msg);
            });
        }

        /// <summary>
        /// 保存消息
        /// </summary>
        private void SaveMsg(string channel, string msg)
        {
            
            //记录消息 只保留500条数据
            var key = $"pubsub_{channel}_msgs";
            _redisDb.ListLeftPush(key, msg);
            if (_redisDb.ListLength(key) > 500)
            {
                _redisDb.ListLeftPop(key);
            }

            _redisDb.KeyExpire(key, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        public List<string> GetMsg(string channel,string userId)
        {
            return new List<string>();
        }

        #endregion

        #region 渠道管理

        /// <summary>
        /// 获取渠道
        /// </summary>
        public List<string> GetChannelList()
        {
            return _redisDb.SetMembers(_channelListKey).Select(t=> t.ToString()).ToList();
        }

        /// <summary>
        /// 获取渠道
        /// </summary>
        public bool CheckChannel(string value)
        {
            return _redisDb.SetContains(_channelListKey, value);
        }

        /// <summary>
        /// 添加渠道
        /// </summary>
        /// <param name="channel"></param>
        public void AddChannel(string channel)
        {
            _redisDb.SetAdd(_channelListKey, channel);
            _redisDb.Publish("add_channel", channel);
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        /// <param name="channel"></param>
        public void DelChannel(string channel)
        {
            _redisDb.SetRemove(_channelListKey, channel);
            _redisDb.Publish("add_channel", channel);
        }

        #endregion

        #region 渠道用户管理

        /// <summary>
        /// 获取订阅用户
        /// </summary>
        /// <param name="channel"></param>
        public List<string> GetChannelSubscribeUser(string channel)
        {
            var key = $"pubsub_{channel}_userids";
            return _redisDb.SetMembers(key).Select(t => t.ToString()).ToList();
        }

        /// <summary>
        /// 添加订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public void AddChannelSubscribeUser(string channel, string userId)
        {
            var key = $"pubsub_{channel}_userids";
            _redisDb.SetAdd(key, userId);
        }

        /// <summary>
        /// 删除订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public void DelChannelSubscribeUser(string channel, string userId)
        {
            var key = $"pubsub_{channel}_userids";
            _redisDb.SetRemove(key, userId);
        }

        #endregion

    }
}
