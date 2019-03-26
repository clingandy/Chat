using System;

namespace ChatWeb.Model
{
    [Serializable]
    public class UserEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public bool IsOnLine { get; set; }
        
    }
}
