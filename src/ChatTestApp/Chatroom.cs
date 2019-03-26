using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading.Tasks.Dataflow;
using ChatTestApp.Model;
using ChatTestApp.Tool;

namespace ChatTestApp
{
    public partial class Chatroom : Form
    {
        #region 基础

        int _threadSleepTime = 500; //线程睡眠时间
        int _addThreadTestCount;    //线程添加的数量
        int _roomUserCount; //房间用户数量
        volatile bool _isSendMsgTest = false;   //是否发送测试消息
        volatile bool _isAddThreadTest = false;  //是否添加连接数
        readonly string _channelName;
        readonly string _userId;
        readonly string _userName;

        private readonly ActionBlock<string> _reMsgActionBlockBatch;
        private ClientWebSocket _clientWebSocket;
        readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public Chatroom(string channelName, string userId, string userName)
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
            _userName = userName;
            _channelName = channelName;
            _addThreadTestCount = 0;

            //初始化队列
            _reMsgActionBlockBatch = new ActionBlock<string>(msg => { MsgHandle(msg); });

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
            if (AppConfig.DicOpenForms.ContainsKey(_channelName))
            {
                AppConfig.DicOpenForms.TryRemove(_channelName, out _);
            }
        }

        private void Chatroom_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isAddThreadTest = false;
            _tokenSource.Cancel();
        }

        #endregion

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
                    FromId = _userId
                }.JsonSerialize();

                //API发送消息
                //var task = new Task(() =>
                //{
                //    Utils.GetData($"{AppConfig.Url}/SendMsg?channel={_channelName}&msg={content}");
                //    //ShowMsg(msg);
                //});
                //task.Start();

                //WebSocket发送消息
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(content));
                _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

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
                    ShowMsg("WebSocker连接服务器开始", isTest);
                    var ws = new ClientWebSocket();
                    await ws.ConnectAsync(new Uri($"{AppConfig.WebSocketUrl}?{channelName}?{userId}?{_userName}"), CancellationToken.None);
                    ShowMsg("WebSocker连接服务器成功", isTest);
                    if (!isTest)
                    {
                        _clientWebSocket = ws;
                    } 
                    while (true)
                    {
                        var buffer = new ArraySegment<byte>(new byte[1024]);
                        await ws.ReceiveAsync(buffer, CancellationToken.None); //接受数据
                        var msgStr = Encoding.UTF8.GetString(Utils.RemoveSeparator(buffer.ToArray()));
                        if (!isTest)
                        {
                            _reMsgActionBlockBatch.Post(msgStr);
                        }
                    }
                }
                catch (Exception e)
                {
                    ShowMsg("WebSocker出错：" + e.Message, isTest);
                    //重连
                    Thread.Sleep(5000);
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
                        ShowMsg($"{msgEntity.FromName}:{msgEntity.Data}");
                        break;
                    case (int)MsgTypeEnum.登出:
                        if (_listBoxUserList.Items.Contains(msgEntity.FromId))
                        {
                            _listBoxUserList.Items.Remove(msgEntity.FromId);
                            _listBoxUserList.Update();
                            ComputeRoomUserCount(-1);
                        }
                        ShowUserOnlineCount(msgEntity.Data);
                        break;
                    case (int) MsgTypeEnum.登录:
                        if (!_listBoxUserList.Items.Contains(msgEntity.FromId))
                        {
                            _listBoxUserList.Items.Add(msgEntity.FromId);
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
            if (isTest || string.IsNullOrWhiteSpace(msg))
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

        private void _txtThreadSleepTime_Leave(object sender, EventArgs e)
        {
            if (Int32.TryParse(_txtThreadSleepTime.Text, out var threadSleepTime))
            {
                _threadSleepTime = threadSleepTime;
            }
        }

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

            var msgModel = new MsgEntity
            {
                Type = (int) MsgTypeEnum.文本,
                Data = "",
                FromId = _userId
            };
            Task.Factory.StartNew(async () =>
            {
                var count = 50;
                while (count-- > 0)
                {
                    msgModel.Data = $"这是一条测试消息！{_threadSleepTime}毫秒发送一次，共50次，次数：{count}";
                    //var task = new Task(() =>
                    //{
                    //    Utils.GetData($"{AppConfig.Url}/SendMsg?channel={_channelName}&msg={msgModel.JsonSerialize()}");
                    //});
                    //task.Start();
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msgModel.JsonSerialize()));
                    await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    Thread.Sleep(_threadSleepTime);
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
                        Thread.Sleep(_threadSleepTime);
                        continue;
                    }
                    _addThreadTestCount++;
                    var userId = Guid.NewGuid().ToString().Replace("-", "");
                    GetMsgByWebSocket(_channelName, userId, true);
                    _btnAddThread.Text = _isAddThreadTest ? $"停止增加连接数:{_addThreadTestCount}" : $"开始增加连接数:{_addThreadTestCount}";
                    Thread.Sleep(_threadSleepTime);
                }
            }, _tokenSource.Token);
        }



        #endregion

    }
}
