
using System;

namespace ChatTestApp.Model
{
    [Serializable]
    public class UserEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public bool IsOnLine { get; set; }

        public int UnreadMsgCount { get; set; }

        public string DisplayName { get; set; }
    }
}
