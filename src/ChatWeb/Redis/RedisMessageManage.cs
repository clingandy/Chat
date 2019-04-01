using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatWeb.Enum;
using ChatWeb.Model;
using ChatWeb.Tool;
using RedisAccessor;

namespace ChatWeb.Redis
{
    public class RedisMessageManage
    {
        #region 属性、构造

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
        public void SendMsg(string channel, MsgEntity msg, bool isSave = false)
        {
            Task.Factory.StartNew(() =>
            {
                _redisHelper.Publish(channel, msg.JsonSerialize());
                // 保存消息
                if (isSave)
                {
                    SaveMsg(channel, msg);
                }
                
            });
        }

        /// <summary>
        /// 保存消息 左进
        /// </summary>
        private Task SaveMsg(string channel, MsgEntity msg)
        {
            var key = $"msgs_{channel}";
            return _redisHelper.ListLeftPushAsync(key, msg);
            //_redisHelper.KeyExpire(key, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 消息出栈 右出
        /// </summary>
        public Task<MsgEntity> GetNextMsg(string channel)
        {
            var key = $"msgs_{channel}";
            return _redisHelper.ListRightPopAsync<MsgEntity>(key);
        }

        /// <summary>
        /// 全部消息
        /// </summary>
        public Task<List<MsgEntity>> GetMsgList(string channel)
        {
            var key = $"msgs_{channel}";
            return _redisHelper.ListRangeAsync<MsgEntity>(key);
        }

        /// <summary>
        /// 删除全部消息
        /// </summary>
        public Task DelAllMsg(string channel)
        {
            var key = $"msgs_{channel}";
            return _redisHelper.KeyDeleteAsync(key);
        }

        #endregion

        #region 渠道管理（聊天室）

        private readonly string _channelListKey = "channels";

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
        public Task AddChannel(string channel)
        {
            return _redisHelper.SetAddAsync(_channelListKey, channel);
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        /// <param name="channel"></param>
        public Task DelChannel(string channel)
        {
            return _redisHelper.SetRemoveAsync(_channelListKey, channel);
        }

        #endregion

        #region 渠道用户管理（聊天室）

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
        public Task AddChannelSubscribeUser(string channel, string userId)
        {
            var key = $"{channel}_userids";
            return _redisHelper.SetAddAsync(key, userId);

        }

        /// <summary>
        /// 删除订阅用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        public Task DelChannelSubscribeUser(string channel, string userId)
        {
            var key = $"{channel}_userids";
            return _redisHelper.SetRemoveAsync(key, userId);
        }

        #endregion

        #region 用户管理（当数据库用，用户好友关系不处理，直接获取全部用户当好友）测试用

        public readonly string RedisUserKey = "User";

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        public async void AddUser(UserEntity user)
        {
            var isExists = await _redisHelper.HashExistsAsync(RedisUserKey, user.UserName);
            if (!isExists)
            {
                await _redisHelper.HashSetAsync(RedisUserKey, user.UserName, user);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        public async void DelUser(UserEntity user)
        {
            await _redisHelper.HashDeleteAsync(RedisUserKey, user.UserName);
        }

        /// <summary>
        /// 获取用户 不存在添加
        /// </summary>
        /// <param name="userName"></param>
        public async Task<UserEntity> GetUser(string userName)
        {
            var isExists = await _redisHelper.HashExistsAsync(RedisUserKey, userName);
            if (!isExists)
            {
                var user = new UserEntity()
                {
                    Status = (int)ClientStatusEnum.OnLine,
                    UserId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                    UserName = userName
                };
                await _redisHelper.HashSetAsync(RedisUserKey, user.UserName, user);
                return user;
            }
            return await _redisHelper.HashGetAsync<UserEntity>(RedisUserKey, userName);

        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public async Task<List<UserEntity>> GetUserList()
        {
            return await _redisHelper.HashValuesAsync<UserEntity>(RedisUserKey);
        }

        #endregion

    }
}
