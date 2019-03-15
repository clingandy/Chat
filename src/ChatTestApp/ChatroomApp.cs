using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatTestApp.Tool;
using Exception = System.Exception;

namespace ChatTestApp
{
    public partial class ChatroomApp : Form
    {
        private readonly string _userId;

        public ChatroomApp()
        {
            InitializeComponent();

            //ClientSocket.Instance.onGetReceive = ShowMsg;
            //ClientSocket.Instance.ConnectServer("127.0.0.1", 8078);
            //ClientSocket.Instance.SendMessage("");

            _userId = Guid.NewGuid().ToString().Replace("-","").ToLower();

            // 获取频道
            GetChannelList();

        }

        /// <summary>
        /// 获取频道
        /// </summary>
        private void GetChannelList()
        {
            new Task(() =>
            {
                var channelList = Utils.GetData($"{AppConfig.Url}/GetChannelList").JsonDeserialize<List<string>>();
                _listBoxChannel.Items.Clear();
                foreach (var item in channelList)
                {
                    _listBoxChannel.Items.Add(item);
                }

                _listBoxChannel.Update();
            }).Start();
        }

        /// <summary>
        /// 添加频道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnAddChannel_Click(object sender, EventArgs e)
        {
            var value = _txtAddChannelName.Text.Trim();
            if (string.IsNullOrEmpty(value) || !Regex.IsMatch(value, "^[a-zA-z0-9]+$") || value.Length < 5)
            {
                //MessageBox.Show(@"要添加的领域名称不能为空！并且只能是字母数字");
                return;
            }

            if (string.IsNullOrWhiteSpace(value) || _listBoxChannel.Items.Contains(value))
            {
                return;
            }

            _listBoxChannel.Items.Add(_txtAddChannelName.Text.Trim());

            new Task(() =>
            {
                var msg = Utils.GetData($"{AppConfig.Url}/AddChannel?channel={_txtAddChannelName.Text.Trim()}");
                //ShowMsg(msg);
            }).Start();
        }

        private void _btnRefreshChannel_Click(object sender, EventArgs e)
        {
            GetChannelList();
        }

        private void _btnOpenChannel_Click(object sender, EventArgs e)
        {
            
            OpenChatroom();
        }

        private void _listBoxChannel_DoubleClick(object sender, EventArgs e)
        {
            OpenChatroom();
        }

        /// <summary>
        /// 打开聊天室
        /// </summary>
        private void OpenChatroom()
        {
            var name = _listBoxChannel.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            Thread.Sleep(200);

            try
            {
                if (AppConfig.DicOpenForms.ContainsKey(name))
                {
                    AppConfig.DicOpenForms[name].Activate();
                    return;
                }

                var chatroom = new Chatroom(name, _userId);
                AppConfig.DicOpenForms[name] = chatroom;
                chatroom.Show();
            }
            catch (Exception)
            {
                //Console.WriteLine(e);
                //throw;
            }
            
        }
    }
}
