namespace ChatWeb.Config
{
    public class MessageConfigure
    {
        /// <summary>
        /// 全部渠道限制多少个客户端连接并行发消息
        /// </summary>
        public int TotalMaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// 单个渠道限制多少个客户端连接并行发消息
        /// </summary>
        public int ChannelMaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// 每个客户端连接发消息间隔多久才Next下一个客户端连接发送
        /// </summary>
        public int SendMsgSpanTime { get; set; }

        /// <summary>
        /// 每个渠道消息队列中缓存的最大数量 , 超过后面的消息将丢弃
        /// </summary>
        public int BoundedCapacity { get; set; }
    }
}
