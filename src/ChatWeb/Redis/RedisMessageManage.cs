using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedisAccessor;

namespace ChatWeb.Redis
{
    public class RedisMessageManage
    {
        #region 属性、构造

        private static readonly string _channelListKey = "channels";

        private readonly RedisHelper _redisHelper;

        public RedisMessageManage(RedisHelper redisHelper)
        {
            _redisHelper = redisHelper;
        }

        #endregion

        #region 取消、订阅渠道

        /// <summary>
        /// 订阅渠道 
        /// </summary>
        public void OnSubscribe(string channelName, Action<string> sendMsg)
        {
            _redisHelper.Subscribe(channelName, (channel, value) =>
            {
                sendMsg.Invoke(value.ToString());
            });
        }

        /// <summary>
        /// 取消订阅渠道
        /// </summary>
        public void OnUnSubscribe(string channelName)
        {
            _redisHelper.Unsubscribe(channelName);
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
                _redisHelper.Publish(channel, msg);
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
            var key = $"{channel}_msgs";
            _redisHelper.ListLeftPush(key, msg);
            if (_redisHelper.ListLength(key) > 500)
            {
                _redisHelper.ListRightPop<string>(key);
            }

            _redisHelper.KeyExpire(key, TimeSpan.FromDays(7));
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
            return _redisHelper.SetMembersAsync(_channelListKey).Result.Select(t=> t.ToString()).ToList();
        }

        /// <summary>
        /// 获取渠道
        /// </summary>
        public bool CheckChannel(string value)
        {
            return _redisHelper.SetContains(_channelListKey, value);
        }

        /// <summary>
        /// 添加渠道
        /// </summary>
        /// <param name="channel"></param>
        public void AddChannel(string channel)
        {
            _redisHelper.SetAdd(_channelListKey, channel);
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        /// <param name="channel"></param>
        public void DelChannel(string channel)
        {
            _redisHelper.SetRemove(_channelListKey, channel);
        }

        #endregion

        #region 渠道用户管理

        /// <summary>
        /// 获取订阅用户
        /// </summary>
        /// <param name="channel"></param>
        public List<string> GetChannelSubscribeUser(string channel)
        {
            var key = $"{channel}_userids";
            return _redisHelper.SetMembers(key).Select(t => t.ToString()).ToList();
        }

        /// <summary>
        /// 添加订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public void AddChannelSubscribeUser(string channel, string userId)
        {
            var key = $"{channel}_userids";
            _redisHelper.SetAdd(key, userId);
        }

        /// <summary>
        /// 删除订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public void DelChannelSubscribeUser(string channel, string userId)
        {
            var key = $"{channel}_userids";
            _redisHelper.SetRemove(key, userId);
        }

        #endregion

    }
}
