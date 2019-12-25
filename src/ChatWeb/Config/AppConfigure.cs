namespace ChatWeb.Config
{
    public class MessageConfigure
    {
        public int TotalMaxDegreeOfParallelism { get; set; }

        public int ChannelMaxDegreeOfParallelism { get; set; }

        public int SendMsgSpanTime { get; set; }

        public int BoundedCapacity { get; set; }
    }
}
