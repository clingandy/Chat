using System.Collections.Concurrent;
using System.Configuration;

namespace PubSubTestApp.Tool
{
    public static class AppConfig
    {
        public static string Url => ConfigurationSettings.AppSettings["ChatroomWebApiAddr"];
        public static string WebSocketUrl => ConfigurationSettings.AppSettings["ChatroomWebSockerAddr"];


        public static ConcurrentDictionary<string, Chatroom> DicOpenChannel = new ConcurrentDictionary<string, Chatroom>();
    }
}
