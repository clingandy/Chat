using System;
using System.Collections.Generic;
using ChatWeb.Model;
using Microsoft.AspNetCore.Mvc;
using ChatWeb.Redis;
using ChatWeb.Tool;
using Microsoft.Extensions.DependencyInjection;

namespace ChatWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly RedisMessageManage _redisMessageManage;

        public HomeController(IServiceProvider service)
        {
            _redisMessageManage = service.GetService<RedisMessageManage>();
        }

        #region 主页

        public JsonResult Index()
        {
            return new JsonResult("OK");
        }

        #endregion

        #region 渠道管理

        /// <summary>
        /// 获取渠道
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChannelList()
        {
            return new JsonResult(_redisMessageManage.GetChannelList());
        }

        /// <summary>
        /// 添加渠道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public JsonResult AddChannel(string channel)
        {
            if (!string.IsNullOrWhiteSpace(channel))
            {
                _redisMessageManage.AddChannel(channel);
            }
            return new JsonResult("添加渠道OK");
        }

        /// <summary>
        /// 删除渠道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public JsonResult DelChannel(string channel)
        {
            if (!string.IsNullOrWhiteSpace(channel))
            {
                _redisMessageManage.DelChannel(channel);
            }
            return new JsonResult("删除渠道OK");
        }

        /// <summary>
        /// 添加渠道用户
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public JsonResult AddChannelUser(string channel, string userId)
        {
            if (!string.IsNullOrWhiteSpace(channel) && !string.IsNullOrWhiteSpace(userId))
            {
                _redisMessageManage.AddChannelSubscribeUser(channel, userId);
            }
            return new JsonResult("订阅渠道OK");
        }

        /// <summary>
        /// 获取渠道用户列表
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public JsonResult GetChannelUserList(string channel)
        {
            if (!string.IsNullOrWhiteSpace(channel))
            {
                var userList = _redisMessageManage.GetChannelSubscribeUser(channel);
                return new JsonResult(userList);
            }
            return new JsonResult(new List<string>());
        }
        #endregion

        #region 消息管理

        /// <summary>
        /// 发送消息
        /// </summary>
        public JsonResult SendMsg(string channel, string msg)
        {
            _redisMessageManage.SendMsg(channel, msg.JsonDeserialize<MsgEntity>());
            return new JsonResult("发送消息OK");
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        public JsonResult GetMsg(string channel, string userId)
        {
            var msg = _redisMessageManage.GetMsgList(channel);
            return new JsonResult(msg);
        }

        #endregion

        #region 用户管理

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult GetUserByUserName(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userList = _redisMessageManage.GetUser(userName).Result;
                return new JsonResult(userList);
            }
            return new JsonResult(new UserEntity());
        }

        #endregion

    }
}
