using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatTestApp.Model;
using ChatTestApp.Tool;

namespace ChatTestApp
{
    public partial class Chatroom : Form
    {
        int _addThreadTestCount;    //线程添加的数量
        int _roomUserCount; //房间用户数量
        volatile bool _isSendMsgTest = false;   //是否发送测试消息
        volatile bool _isAddThreadTest = false;  //是否添加连接数
        readonly string _channelName;
        readonly string _userId;
        
        readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public Chatroom(string channelName, string userId)
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(channelName))
            {
                channelName = "chatroom";
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = Guid.NewGuid().ToString().Replace("-","").ToLower();
            }
            _userId = userId;
            _channelName = channelName;
            _addThreadTestCount = 0;

            //获取用户列表
            GetUserList();
            // 获取websocker
            GetMsgByWebSocket(_channelName, _userId);
            // 添加连接数
           AddAddThreadTestTask();
        }


        private void _txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            AcceptButton = _btnSendMsg;
        }

        private void Chatroom_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isAddThreadTest = false;
            _tokenSource.Cancel();
            if (AppConfig.DicOpenChannel.ContainsKey(_channelName))
            {
                AppConfig.DicOpenChannel.TryRemove(_channelName, out _);
            }
        }

        private void Chatroom_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isAddThreadTest = false;
            _tokenSource.Cancel();
        }

        #region 操作

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        private void GetUserList()
        {
            _listBoxUserList.Items.Clear();
            _listBoxUserList.Items.Add(_userId);
            Task.Factory.StartNew(() =>
            {
                var userListStr = Utils.GetData($"{AppConfig.Url}/GetChannelUserList?channel={_channelName}");
                var userList = userListStr.JsonDeserialize<List<string>>();
                userList = userList.Where(t => t != _userId).ToList();
                foreach (var item in userList)
                {
                    _listBoxUserList.Items.Add(item);
                }
                _listBoxUserList.Update();
                ShowUserOnlineCount();
                ComputeRoomUserCount(_listBoxUserList.Items.Count);
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnSendMsg_Click(object sender, EventArgs e)
        {
            var content = _txtMsg.Text;
            _txtMsg.Text = "";
            if (!string.IsNullOrWhiteSpace(content))
            {
                content = new MsgEntity
                {
                    Type = (int)MsgTypeEnum.文本,
                    Data = content,
                    Code = 200,
                    UserId = _userId
                }.JsonSerialize();

                var task = new Task(() =>
                {
                    Utils.GetData($"{AppConfig.Url}/SendMsg?channel={_channelName}&msg={content}");
                    //ShowMsg(msg);
                });
                task.Start();

            }
        }

        /// <summary>
        /// WebSocket获取消息
        /// </summary>
        /// <returns></returns>
        private void GetMsgByWebSocket(string channelName, string userId, bool isTest = false)
        {
            Task.Factory.StartNew(async ()=>
            {
                try
                {
                    ShowMsg("WebSocker连接服务器", isTest);
                    var ws = new ClientWebSocket();
                    //ws.Options.AddSubProtocol(channelName);
                    await ws.ConnectAsync(new Uri($"{AppConfig.WebSocketUrl}?{channelName}?{userId}"), CancellationToken.None);
                    ShowMsg("WebSocker连接服务器成功", isTest);
                    while (true)
                    {
                        var result = new byte[1024];
                        await ws.ReceiveAsync(new ArraySegment<byte>(result), CancellationToken.None); //接受数据
                        var msgStr = Encoding.UTF8.GetString(result, 0, result.Length);
                        if (!isTest)
                        {
                            MsgHandle(msgStr.Replace("\\0", ""));    //处理空字符
                        }
                    }
                }
                catch (Exception e)
                {
                    ShowMsg("WebSocker出错：" + e.Message, isTest);
                    //重连
                    Thread.Sleep(10000);
                    GetMsgByWebSocket(channelName, userId, isTest);
                }
            });
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="msgStr"></param>
        private void MsgHandle(string msgStr)
        {
            try
            {
                var msgEntity = msgStr.JsonDeserialize<MsgEntity>();
                switch (msgEntity.Type)
                {
                    case (int)MsgTypeEnum.文本:
                        ShowMsg($"{msgEntity.UserId}:{msgEntity.Data}");
                        break;
                    case (int)MsgTypeEnum.登出:
                        if (_listBoxUserList.Items.Contains(msgEntity.UserId))
                        {
                            _listBoxUserList.Items.Remove(msgEntity.UserId);
                            _listBoxUserList.Update();
                            ComputeRoomUserCount(-1);
                        }
                        ShowUserOnlineCount(msgEntity.Data);
                        break;
                    case (int) MsgTypeEnum.登录:
                        if (!_listBoxUserList.Items.Contains(msgEntity.UserId))
                        {
                            _listBoxUserList.Items.Add(msgEntity.UserId);
                            _listBoxUserList.Update();
                            ComputeRoomUserCount(1);
                        }
                        ShowUserOnlineCount(msgEntity.Data);
                        break;
                }
            }
            catch (Exception)
            {
                ShowMsg(msgStr);
            }
            
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        private void ShowMsg(string msg, bool isTest = false)
        {
            if (isTest || string.IsNullOrWhiteSpace(msg) || msg == "[]")
            {
                return;
            }
            _listBoxMsg.Items.Add(msg);
            if (_listBoxMsg.Items.Count > 30)
            {
                _listBoxMsg.Items.RemoveAt(0);
            }
            _listBoxMsg.Update();
        }

        /// <summary>
        /// 显示用户在线人数，和活动连接数量
        /// </summary>
        private void ShowUserOnlineCount(string activityCount = "0")
        {
            Text = $@"Chatroom—{_channelName}—房间人数：{_listBoxUserList.Items.Count}-活动人数：{activityCount}";
            Update();
        }

        /// <summary>
        /// 统计房间人数
        /// </summary>
        /// <param name="num"></param>
        private void ComputeRoomUserCount(int num)
        {
            Interlocked.Add(ref _roomUserCount, num);
        }

        #endregion


        #region 连接数并发、消息并发

        private void _btnAddThread_Click(object sender, EventArgs e)
        {
            _isAddThreadTest = !_isAddThreadTest;
            _btnAddThread.Text = _isAddThreadTest ? $"停止增加连接数:{_addThreadTestCount}" : $"开始增加连接数:{_addThreadTestCount}";
        }

        private void _btnSendMsgTest_Click(object sender, EventArgs e)
        {
            if (_isSendMsgTest)
            {
                return;
            }
            _isSendMsgTest = true;
            _btnSendMsgTest.Text = @"并发消息测试...";
            Task.Factory.StartNew(() =>
            {
                var count = 100;
                while (count-- > 0)
                {
                    var content = $"这是一条测试消息！100毫秒发送一次，共100次，次数：{count}";
                    var task = new Task(() =>
                    {
                        Utils.GetData($"{AppConfig.Url}/SendMsg?channel={_channelName}&msg={content}");
                    });
                    task.Start();
                    Thread.Sleep(100);
                }
                _isSendMsgTest = false;
                _btnSendMsgTest.Text = @"并发消息测试";
            });
        }

        private void AddAddThreadTestTask()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!_isAddThreadTest)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }
                    _addThreadTestCount++;
                    var userId = Guid.NewGuid().ToString().Replace("-", "");
                    GetMsgByWebSocket(_channelName, userId, true);
                    _btnAddThread.Text = _isAddThreadTest ? $"停止增加连接数:{_addThreadTestCount}" : $"开始增加连接数:{_addThreadTestCount}";
                    Thread.Sleep(5);
                }
            }, _tokenSource.Token);
        }


        #endregion

        
    }
}
