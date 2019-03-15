using System.Collections.Concurrent;
using System.Configuration;
using System.Windows.Forms;

namespace ChatTestApp.Tool
{
    public static class AppConfig
    {
        public static string Url => ConfigurationSettings.AppSettings["ChatroomWebApiAddr"];
        public static string WebSocketUrl => ConfigurationSettings.AppSettings["ChatroomWebSockerAddr"];


        public static ConcurrentDictionary<string, Form> DicOpenForms = new ConcurrentDictionary<string, Form>();
    }
}
