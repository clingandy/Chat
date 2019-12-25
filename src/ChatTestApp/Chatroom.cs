using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading.Tasks.Dataflow;
using ChatTestApp.Model;
using ChatTestApp.Tool;
using ChatTestApp.WebSocket;

namespace ChatTestApp
{
    public partial class Chatroom : Form
    {
        #region 基础

        int _addThreadTestTotalCount;    //线程添加的数量
        int _disconnectThreadTestCount;  //线程断开的数量
        volatile bool _isSendMsgTest;   //是否发送测试消息
        readonly string _channelName;
        readonly string _userId;
        readonly string _userName;

        private readonly ActionBlock<string> _reMsgActionBlockBatch;
        //private ClientWebSocket _clientWebSocket;
        private WebSocketHelper _webSocketHelper;
        readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private string _preMsgId;   //上一个消息ID
        private ConcurrentDictionary<string, int> _receiveMsgTotalCountDic = new ConcurrentDictionary<string, int>();

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
            _addThreadTestTotalCount = _disconnectThreadTestCount = 0;

            //初始化队列
            _reMsgActionBlockBatch = new ActionBlock<string>(msg => { MsgDeserializeAndHandle(msg); });

            // 获取websocker
            GetMsgByWebSocket(_channelName, _userId);

            //获取用户列表
            // GetUserList();

        }

        private void _txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            AcceptButton = _btnSendMsg;
        }

        private void _btnClearMsg_Click(object sender, EventArgs e)
        {
            ClearListBox();
        }

        private void _btnShowReceiveInfo_Click(object sender, EventArgs e)
        {
            _listBoxReceiveInfo.Items.Clear();

            foreach (var item in _receiveMsgTotalCountDic)
            {
                _listBoxReceiveInfo.Items.Add($"消息数量：{item.Value}");
            }
            _listBoxReceiveInfo.Update();
        }

        private void Chatroom_FormClosed(object sender, FormClosedEventArgs e)
        {
            _tokenSource.Cancel();
            if (AppConfig.DicOpenForms.ContainsKey(_channelName))
            {
                AppConfig.DicOpenForms.TryRemove(_channelName, out _);
            }
        }

        private void Chatroom_FormClosing(object sender, FormClosingEventArgs e)
        {
            _tokenSource.Cancel();
        }

        #endregion

        #region 操作

        private void ClearListBox()
        {
            _receiveMsgTotalCountDic.Clear();
            _listBoxMsg.Items.Clear();
            //while (_listBoxMsg.Items.Count > 3)
            //{
            //    _listBoxMsg.Items.RemoveAt(0);
            //}
            _listBoxMsg.Update();
        }

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
                ShowUserCount();
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnSendMsg_Click(object sender, EventArgs e)
        {
            try
            {
                var content = _txtMsg.Text;
                _txtMsg.Text = "";
                if (!string.IsNullOrWhiteSpace(content))
                {
                    content = new MsgEntity
                    {
                        MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                        Type = (int)MsgTypeEnum.文本,
                        Data = content,
                        FromId = _userId,
                        FromName = ""
                    }.JsonSerialize();

                    _webSocketHelper?.SendMsg(content);
                }
            }
            catch (Exception exception)
            {
                //Console.WriteLine(exception);
                //throw;
                ShowMsg("发送消息出错：" + exception.Message);
            }
            
        }

        /// <summary>
        /// WebSocket获取消息
        /// </summary>
        /// <returns></returns>
        private void GetMsgByWebSocket(string channelName, string userId, bool isTest = false)
        {
            ShowMsg("WebSocker连接服务器开始", isTest);

            var webSocketHelper = new WebSocketHelper(channelName, userId, _userName, isTest);
            if (!isTest)
            {
                _webSocketHelper = webSocketHelper;
            }

            webSocketHelper.EventError += e =>
            {
                ShowMsg("主WebSocker出错：" + e.Message);
            };
            webSocketHelper.EventTestError += e =>
            {
                Interlocked.Add(ref _disconnectThreadTestCount, 1);
                ShowMsg($"后台并发测试WebSocker断开 总数量：{_disconnectThreadTestCount}，错误信息：{(e.InnerException != null ? e.InnerException.Message : e.Message)}");
            };
            webSocketHelper.EventReceiveMsg += msgStr =>
            {
                _reMsgActionBlockBatch.Post(msgStr);
                if (!isTest)
                {
                    ShowUserCount();
                }
            };
            webSocketHelper.ConnServer();
            Interlocked.Add(ref _addThreadTestTotalCount, 1);

            ShowMsg("WebSocker连接服务器成功", isTest);
        }

        /// <summary>
        /// 消息系列化和处理
        /// </summary>
        /// <param name="msgStr"></param>
        private void MsgDeserializeAndHandle(string msgStr)
        {
            try
            {
                var msgEntitys = msgStr.JsonDeserialize<MsgEntity[]>();
                foreach (var msgEntity in msgEntitys)
                {
                    MsgHandle(msgEntity);
                }
            }
            catch (Exception ex)
            {
                //ShowMsg("系列化错误信息："+ ex.Message);
                //ShowMsg(msgStr);
            }
            
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="model"></param>
        private void MsgHandle(MsgEntity model)
        {
            try
            {
                // 计算多少个线程收到消息
                if(_receiveMsgTotalCountDic.AddOrUpdate(model.MsgId, 1, (s, i) => i + 1) != 1) return;

                switch (model.Type)
                {
                    case (int)MsgTypeEnum.文本:
                        ShowMsg($"{model.MsgId}-{model.FromName}-{model.CurTime}：{model.Data}");
                        break;
                    case (int)MsgTypeEnum.登出:
                        //if (_listBoxUserList.Items.Contains(msgEntity.FromId))
                        //{
                        //    _listBoxUserList.Items.Remove(msgEntity.FromId);
                        //    _listBoxUserList.Update();
                        //    ComputeRoomUserCount(-1);
                        //}
                        break;
                    case (int)MsgTypeEnum.登录:
                        //if (!_listBoxUserList.Items.Contains(msgEntity.FromId))
                        //{
                        //    _listBoxUserList.Items.Add(msgEntity.FromId);
                        //    _listBoxUserList.Update();
                        //    ComputeRoomUserCount(1);
                        //}
                        break;
                    case (int)MsgTypeEnum.系统:
                        ShowMsg($"系统消息-{model.CurTime}：{model.Data}");
                        break;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(e);
                //throw;
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
            _listBoxMsg.TopIndex = _listBoxMsg.Items.Count - _listBoxMsg.Height / _listBoxMsg.ItemHeight;
            _listBoxMsg.Update();
        }

        /// <summary>
        /// 显示用户在线人数，和活动连接数量
        /// </summary>
        private void ShowUserCount()
        {
            //Text = $@"Chatroom—{_channelName}—房间人数：{_listBoxUserList.Items.Count}-活动人数：{activityCount}";
            Text = $@"Chatroom-{_channelName}-添加的总数：{_addThreadTestTotalCount}-断开数：{_disconnectThreadTestCount}";
            Update();
        }

        #endregion

        #region 连接数并发、消息并发

        private void _txtThreadSleepTime_Leave(object sender, EventArgs e)
        {
            
        }

        private void _btnAddThread_Click(object sender, EventArgs e)
        {
            _btnAddThread.Enabled = false;
            _btnAddThread.Update();
            if (Int32.TryParse(_txtAddThreadCount.Text, out var addThreadCount))
            {
                Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < addThreadCount; i++)
                    {
                        Thread.Sleep(1);
                        var userId = Guid.NewGuid().ToString().Replace("-", "");
                        GetMsgByWebSocket(_channelName, userId, true);
                    }

                    ShowMsg(@"添加完成.....");
                    _btnAddThread.Enabled = true;
                    _btnAddThread.Update();
                }, _tokenSource.Token);
            }
        }

        private void _btnSendMsgTest_Click(object sender, EventArgs e)
        {
            if (_isSendMsgTest)
            {
                return;
            }
            _isSendMsgTest = true;
            _btnSendMsgTest.Enabled = false;
            _btnSendMsgTest.Text = @"并发消息测试...";

            if (Int32.TryParse(_txtThreadSleepTime.Text, out var threadSleepTime))
            {
                var msgModel = new MsgEntity
                {
                    MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                    Type = (int)MsgTypeEnum.文本,
                    Data = "",
                    FromId = _userId,
                    FromName = ""

                };

                Task.Factory.StartNew(() =>
                {
                    var count = 10;
                    if (Int32.TryParse(_txtSendMsgCount.Text, out var sendMsgCount))
                    {
                        count = sendMsgCount;
                    }

                    while (_isSendMsgTest && count-- > 0)
                    {
                        msgModel.MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower();
                        msgModel.Data = $"这是一条测试消息！{threadSleepTime}毫秒发送一次，共{sendMsgCount}次，次数：{count}";
                        //var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msgModel.JsonSerialize()));
                        //await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        _webSocketHelper.SendMsg(msgModel.JsonSerialize());
                        Thread.Sleep(threadSleepTime);

                        if (_checkBoxAutoClear.Checked)
                        {
                            ClearListBox();
                        }
                        
                    }
                    _isSendMsgTest = false;
                    _btnSendMsgTest.Enabled = true;
                    _btnSendMsgTest.Text = @"并发消息测试";
                });
            }

            
        }

        private void _btnStopSendMsgTest_Click(object sender, EventArgs e)
        {
            _isSendMsgTest = false;
            _btnSendMsgTest.Enabled = true;
            _btnSendMsgTest.Text = @"并发消息测试";
        }


        #endregion

        
    }
}
