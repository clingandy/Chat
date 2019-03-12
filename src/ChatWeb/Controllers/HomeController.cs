using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PubSubWeb.Redis;

namespace PubSubWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        #region 主页

        public JsonResult Index()
        {
            return new JsonResult("OK");
        }

        #endregion

        #region 渠道、用户管理

        /// <summary>
        /// 获取渠道
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChannelList()
        {
            return new JsonResult(RedisHelper.Instance.GetChannelList());
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
                RedisHelper.Instance.AddChannel(channel);
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
                RedisHelper.Instance.DelChannel(channel);
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
                RedisHelper.Instance.AddChannelSubscribeUser(channel, userId);
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
                var userList = RedisHelper.Instance.GetChannelSubscribeUser(channel);
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
            RedisHelper.Instance.SendMsg(channel, msg);
            return new JsonResult("发送消息OK");
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        public JsonResult GetMsg(string channel, string userId)
        {
            var msg = RedisHelper.Instance.GetMsg(channel, userId);
            return new JsonResult(msg);
        }

        #endregion

    }
}
