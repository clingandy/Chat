
using System;

namespace ChatWeb.Model
{
    [Serializable]
    public class MsgEntity
    {
        /// <summary>
        /// 消息Id uuid等随机串
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 消息类型 MsgTypeEnum
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 发送用户ID
        /// </summary>
        public string FromId { get; set; }

        /// <summary>
        /// 发送用户名称
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 0：点对点个人消息，1：群消息（高级群） （聊天室不需要）
        /// </summary>
        public int Ope { get; set; }

        /// <summary>
        /// 接受用户ID （聊天室不需要）
        /// </summary>
        public string ToId { get; set; }


        /// <summary>
        /// 当前UTC时间戳，从1970年1月1日0点0 分0 秒开始到现在的秒数(String)
        /// </summary>
        public long CurTime { get; set; }

    }

    public enum MsgTypeEnum
    {
        文本 = 100,
        登录 = 210,
        登出 = 220,

        系统 = 300,

        禁止发送 = 400,

        请求添加好友 = 1001,
        通过添加好友 = 1002,
        拒接添加好友 = 1003,

        获取好友数据 = 1111,
        获取聊天室数据 = 1112
    }
}
