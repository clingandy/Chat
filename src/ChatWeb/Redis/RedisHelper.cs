using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PubSubWeb.Tool;
using ServiceStack.Redis;

namespace PubSubWeb.Redis
{
    public class RedisHelper
    {
        #region 属性、构造

        private static readonly string _channelListKey = "pubsub_channels";

        public static readonly RedisHelper Instance = new Lazy<RedisHelper>(() => new RedisHelper()).Value;

        #endregion

        #region 创建、关闭发布服务

        /// <summary>
        /// 创建发布服务
        /// </summary>
        public void CreatePublisherService(string channelName)
        {
            //发布、订阅服务 IRedisPubSubServer
            var pubSubServer = new RedisPubSubServer(RedisService.Instance.pooledRedisClient, channelName)
            {
                OnMessage = (channel, msg) =>
                {

                },
                OnStart = () =>
                {

                },
                OnStop = () =>
                {

                },
                //OnUnSubscribe = (channel) => { },
                OnError = (e) =>
                {
                    //Console.WriteLine(e.Message);
                },
                OnFailover = (s) =>
                {
                    //Console.WriteLine(s);
                }
            };

            //接收消息
            //pubSubServer.OnHeartbeatReceived = () => { Console.WriteLine("OnHeartbeatReceived"); };
            //pubSubServer.OnHeartbeatSent = () => { Console.WriteLine("OnHeartbeatSent"); };
            //pubSubServer.IsSentinelSubscription = true;
            pubSubServer.Start();
        }

        /// <summary>
        /// 关闭发布服务
        /// </summary>
        public void ClosePublisherService(string channelName)
        {
            //发布、订阅服务 IRedisPubSubServer
            var pubSubServer = new RedisPubSubServer(RedisService.Instance.pooledRedisClient, channelName);
            pubSubServer.Stop();
        }

        #endregion

        #region 取消、订阅渠道

        /// <summary>
        /// 订阅渠道 
        /// </summary>
        public void OnSubscribe(string channelName, Action<string> sendMsg)
        {
            using (var consumer = RedisService.Instance.Client)
            {
                //创建订阅
                var subscription = consumer.CreateSubscription();

                //接收消息处理Action
                subscription.OnMessage = (channel, msg) =>
                {
                    sendMsg.Invoke(msg);
                };

                //订阅事件处理Action
                subscription.OnSubscribe = channel =>
                {
                    
                };

                //取消订阅事件处理Action
                subscription.OnUnSubscribe = channel =>
                {
                    OnSubscribe(channelName, sendMsg);
                };

                //订阅渠道
                subscription.SubscribeToChannels(channelName);
            }
        }

        /// <summary>
        /// 取消订阅渠道
        /// </summary>
        public void OnUnSubscribe(string channelName)
        {
            using (var consumer = RedisService.Instance.Client)
            {
                //取消订阅
                var subscription = consumer.CreateSubscription();
                subscription.UnSubscribeFromChannels(channelName);
            }
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
                using (var client = RedisService.Instance.Client)
                {
                    //发送消息
                    client.PublishMessage(channel, msg);
                    //保存消息
                    //SaveMsg(client, channel, msg);
                }
            });
        }

        /// <summary>
        /// 保存消息
        /// </summary>
        private void SaveMsg(IRedisClient client, string channel, string msg)
        {
            //记录消息 只保留500条数据
            var key = $"pubsub_{channel}_msgs";
            client.AddItemToList(key, msg);
            if (client.GetListCount(key) > 500)
            {
                client.RemoveStartFromList(key);
            }
            client.ExpireEntryIn(key, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        public List<string> GetMsg(string channel,string userId)
        {
            using (var gc = RedisService.Instance.Client)
            {
                //var list = gc.GetAllItemsFromList($"pubsub_{channel}_{userId.Replace("-", "")}");
                //return list.JsonSerialize();

                var key = $"pubsub_{channel}_msgs";
                var msg = gc.RemoveStartFromList(key);
                return string.IsNullOrWhiteSpace(msg) ? new List<string>() : new List<string>() {msg};
            }
        }

        #endregion

        #region 渠道管理

        /// <summary>
        /// 获取渠道
        /// </summary>
        public List<string> GetChannelList()
        {
            using (var client = RedisService.Instance.Client)
            {
                return client.GetAllItemsFromSet(_channelListKey).ToList();
            }
        }

        /// <summary>
        /// 获取渠道
        /// </summary>
        public bool CheckChannel(string value)
        {
            using (var client = RedisService.Instance.Client)
            {
                return client.SetContainsItem(_channelListKey, value);
            }
        }

        /// <summary>
        /// 添加渠道
        /// </summary>
        /// <param name="channel"></param>
        public void AddChannel(string channel)
        {
            using (var client = RedisService.Instance.Client)
            {
                client.AddItemToSet(_channelListKey, channel);
                client.PublishMessage("add_channel", channel);
            }
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        /// <param name="channel"></param>
        public void DelChannel(string channel)
        {
            using (var client = RedisService.Instance.Client)
            {
                client.RemoveItemFromSet(_channelListKey, channel);
                client.PublishMessage("del_channel", channel);
            }
        }

        #endregion

        #region 渠道用户管理

        /// <summary>
        /// 获取订阅用户
        /// </summary>
        /// <param name="channel"></param>
        public List<string> GetChannelSubscribeUser(string channel)
        {
            using (var gc = RedisService.Instance.Client)
            {
                var key = $"pubsub_{channel}_userids";
                return gc.GetAllItemsFromSortedSet(key);
            }
        }

        /// <summary>
        /// 添加订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public void AddChannelSubscribeUser(string channel, string userId)
        {
            using (var gc = RedisService.Instance.Client)
            {
                var key = $"pubsub_{channel}_userids";
                gc.AddItemToSortedSet(key, userId, DateTime.Now.Ticks);
                //gc.ExpireEntryIn(key, TimeSpan.FromHours(6));
            }
        }

        /// <summary>
        /// 删除订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public void DelChannelSubscribeUser(string channel, string userId)
        {
            using (var gc = RedisService.Instance.Client)
            {
                var key = $"pubsub_{channel}_userids";
                gc.RemoveItemFromSortedSet(key, userId);
                //gc.ExpireEntryIn(key, TimeSpan.FromHours(6));
            }
        }

        #endregion

    }
}
