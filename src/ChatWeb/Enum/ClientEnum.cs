using System.ComponentModel;

namespace ChatWeb.Enum
{
    public enum ClientStatusEnum
    {
        [Description("在线")]
        OnLine = 1,
        [Description("下线")]
        SignOut,
        [Description("禁言")]
        Mute
    }
}
