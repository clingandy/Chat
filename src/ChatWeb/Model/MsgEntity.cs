
using System;

namespace PubSubWeb.Model
{
    [Serializable]
    public class MsgEntity
    {
        /// <summary>
        /// 消息类型 MsgTypeEnum
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

    }

    public enum MsgTypeEnum
    {
        文本 = 100,
        登录 = 210,
        登出 = 220,
    }
}
