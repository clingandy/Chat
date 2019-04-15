using System;
using System.Windows.Forms;

namespace ChatTestApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatApp());

            //Application.Run(new Chatroom("chatroom", Guid.NewGuid().ToString().Replace("-", "").ToLower(), "zhangsan"));
        }
    }
}
