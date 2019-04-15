using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatTestApp.Model;
using ChatTestApp.Tool;
using ChatTestApp.WebSocket;


namespace ChatTestApp
{
    public partial class ChatApp : Form
    {
        private string _userId;
        private string _userName;

        private string _curToId;    //当前选择用户ID
        private string _sysId;      //系统ID
        private bool _isAddFriend = false;  //是否已经添加朋友列表
        private WebSocketHelper _webSocketHelper;

        private BindingList<UserEntity> _userList;
        private ConcurrentDictionary<string, BindingList<string>> _dicMsgList;
        
        // 调试模式需要用下面对象更新UI，目前只可以在非调试模式运行
        //private SynchronizationContext _synchronization;

        public ChatApp()
        {
            InitializeComponent();

            //ClientSocket.Instance.onGetReceive = ShowMsg;
            //ClientSocket.Instance.ConnectServer("127.0.0.1", 8078);
            //ClientSocket.Instance.SendMessage("");

            // _synchronization = SynchronizationContext.Current;

            _tabPageChat.Enabled = false;
            _tabPageChatroom.Enabled = false;
            _tabPageLogin.Enabled = true;
            _tabControlChat.SelectedIndex = 2;

        }

        #region Chatroom

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
            var channelName = _listBoxChannel.Text;
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }
            Thread.Sleep(200);

            try
            {
                if (AppConfig.DicOpenForms.ContainsKey(channelName))
                {
                    AppConfig.DicOpenForms[channelName].Activate();
                    return;
                }

                var chatroom = new Chatroom(channelName, _userId, _userName);
                AppConfig.DicOpenForms[channelName] = chatroom;
                chatroom.Show();
            }
            catch (Exception)
            {
                //Console.WriteLine(e);
                //throw;
            }
            
        }

        #endregion

        #region IM

        /// <summary>
        /// 连接webSocket ，个人的渠道就是自己的ID
        /// </summary>
        private void ConnWebSocket()
        {
            _dicMsgList[_sysId].Add("WebSocker连接服务器开始");

            _webSocketHelper = new WebSocketHelper(_userId, _userId, _userName, false);
            _webSocketHelper.EventError += e =>
            {
                _dicMsgList[_sysId].Add($"WebSocker连接出错，错误信息：{e.Message}");
            };
            _webSocketHelper.EventReceiveMsg += MsgHandle;

            _webSocketHelper.ConnServer();

            _dicMsgList[_sysId].Add("WebSocker连接服务器成功");

        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnAddUser_Click(object sender, EventArgs e)
        {
            var name = _txtAddUserName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, "^[a-zA-z\u4e00-\u9fa5]+$") && _userList.Count(t => t.UserName == name) == 0)
            {
                var newUserId = Guid.NewGuid().ToString().Replace("-", "").ToLower();

                var content = new MsgEntity
                {
                    Type = (int)MsgTypeEnum.请求添加好友,
                    Data = name,
                    FromId = _userId,
                    ToId = newUserId
                }.JsonSerialize();

                //WebSocket发送消息
                _webSocketHelper.SendMsg(content);

                ShowMsg(_sysId, $"你发送消息请求添加[{name}]为好友，默认直接添加好友");


                _userList.Add(new UserEntity()
                {
                    UserId = newUserId,
                    UserName = name,
                    DisplayName = $"{name}"
                });
            } 
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnSendMsg_Click(object sender, EventArgs e)
        {
            var msg = _txtMsg.Text;
            var curToId = _curToId;
            if (!string.IsNullOrWhiteSpace(msg) && _sysId != curToId)
            {
                _txtMsg.Text = "";
                var content = new MsgEntity
                {
                    MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                    Type = (int)MsgTypeEnum.文本,
                    Data = msg,
                    FromId = string.Empty,  //服务端处理
                    FromName = string.Empty,  //服务端处理
                    ToId = curToId
                }.JsonSerialize();

                //WebSocket发送消息
                _webSocketHelper?.SendMsg(content);

                ShowMsg(curToId, $"{_userName}：{msg}");
            }
            
        }

        private void _txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            AcceptButton = _btnSendMsg;
        }

        private void _txtAddUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            AcceptButton = _btnAddUser;
        }

        private void _listBoxUser_Click(object sender, EventArgs e)
        {
            var obj = _listBoxUser.SelectedValue;

            if (!string.IsNullOrEmpty(obj?.ToString()) && obj.ToString() != _curToId)
            {
                var tempId = obj.ToString();
                if (!_dicMsgList.ContainsKey(tempId))
                {
                    _dicMsgList[tempId] = new BindingList<string>();
                }

                _curToId = tempId;
                _listBoxMsgs.DataSource = _dicMsgList[tempId];

                SetUnreadMsg(tempId, true);

                ScrollListBoxMsgs(true);
            }
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="msgStr"></param>
        private void MsgHandle(string msgStr)
        {
            //ShowMsg(_sysId, msgStr);
            try
            {
                var msgEntitys = msgStr.JsonDeserialize<MsgEntity[]>();
                foreach (var msgEntity in msgEntitys)
                {
                    switch (msgEntity.Type)
                    {
                        case (int)MsgTypeEnum.文本:
                            ShowMsg(msgEntity.FromId, $"{msgEntity.FromName}-{msgEntity.CurTime}：{msgEntity.Data}");
                            break;
                        case (int)MsgTypeEnum.登出:

                            break;
                        case (int)MsgTypeEnum.登录:

                            break;
                        case (int)MsgTypeEnum.系统:
                            ShowMsg(_sysId, $"系统消息-{msgEntity.CurTime}：{msgEntity.Data}");
                            break;
                        case (int)MsgTypeEnum.请求添加好友:
                            ShowMsg(_sysId, $"{msgEntity.FromName}：请求添加为好友");
                            break;
                        case (int)MsgTypeEnum.拒接添加好友:
                            ShowMsg(_sysId, $"{msgEntity.FromName}：拒接你的添加好友申请");
                            break;
                        case (int)MsgTypeEnum.获取好友数据:
                            if (!_isAddFriend)
                            {
                                var userList = msgEntity.Data.JsonDeserialize<List<UserEntity>>();
                                userList = userList.Where(t => t.UserId != _userId).ToList();
                                foreach (var item in userList)
                                {
                                    item.DisplayName = item.UserName;
                                    _userList.Add(item);
                                }

                                _isAddFriend = true;
                            }
                            break;
                        case (int)MsgTypeEnum.获取聊天室数据:
                            ShowMsg(_sysId, "获取聊天室数据成功");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMsg(_sysId, "错误信息：" + ex.Message);
                ShowMsg(_sysId, msgStr);
            }

        }

        /// <summary>
        /// 显示消息  并发考虑
        /// </summary>
        private void ShowMsg(string fromId,string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            if (!_dicMsgList.ContainsKey(fromId))
            {
                _dicMsgList[fromId] = new BindingList<string>();
            }

            _dicMsgList[fromId].Add(msg);   //添加数据

            if (fromId != _curToId)
            {
                SetUnreadMsg(fromId, false);
            }
            else
            {
                ScrollListBoxMsgs(false);
            }
        }

        /// <summary>
        /// 未读消息处理  并发考虑
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isReset"></param>
        private void SetUnreadMsg(string userId, bool isReset)
        {
            var user = _userList.FirstOrDefault(t => t.UserId == userId);
            if (user == null)
            {
                user = new UserEntity()
                {
                    UserId = userId,
                    UserName = userId,
                    DisplayName = "新用户"
                };
                _userList.Add(user);
            }
            if(isReset)
            {
                user.UnreadMsgCount = 0;
                user.DisplayName = $"{user.UserName}";
            }
            else
            {
                user.UnreadMsgCount++;
                user.DisplayName = $"{user.UserName}：{user.UnreadMsgCount}";
            }

            _userList.ResetItem(_userList.IndexOf(user));
            //_userList.ResetBindings();
            //_listBoxUser.SelectedValue = userId;
    
        }

        /// <summary>
        /// 滚动条
        /// </summary>
        /// <param name="bottom">true 直接到底部， false 智能滚动，在底部时候才滚动</param>
        private void ScrollListBoxMsgs(bool bottom)
        {
            //var scroll = _listBoxMsgs.TopIndex == _listBoxMsgs.Items.Count - _listBoxMsgs.Height / _listBoxMsgs.ItemHeight + 1;
            //if (scroll || bottom)
            //{
            //    _listBoxMsgs.TopIndex = _listBoxMsgs.Items.Count - _listBoxMsgs.Height / _listBoxMsgs.ItemHeight;
            //}
            _listBoxMsgs.TopIndex = _listBoxMsgs.Items.Count - _listBoxMsgs.Height / _listBoxMsgs.ItemHeight;
        }

        #endregion

        #region 登录

        private void _txtLoginName_KeyPress(object sender, KeyPressEventArgs e)
        {
            AcceptButton = _btnLogin;
        }

        private void _btnLogin_Click(object sender, EventArgs e)
        {
            var userName = _txtLoginName.Text.Trim();
            if (string.IsNullOrWhiteSpace(userName) || !Regex.IsMatch(userName, "^[a-zA-z\u4e00-\u9fa5]+$"))
            {
                _lblLoginMsg.Text = @"登录失败！登录名称必须英文或汉字";
                _lblLoginMsg.Update();
                return;
            }

            UserEntity user;
            try
            {
                user = Utils.GetData($"{AppConfig.Url}/GetUserByUserName?userName={userName}").JsonDeserialize<UserEntity>();
            }
            catch
            {
                _lblLoginMsg.Text = @"请求登录失败！";
                _lblLoginMsg.Update();
                return;
            }

            if (string.IsNullOrEmpty(user?.UserId))
            {
                _lblLoginMsg.Text = @"请求登录失败！";
                _lblLoginMsg.Update();
                return;
            }
            _lblLoginMsg.Text = "";
            _lblLoginMsg.Update();


            _tabPageChat.Enabled = true;
            _tabPageChatroom.Enabled = true;
            _tabPageLogin.Enabled = false;
            _tabControlChat.SelectedIndex = 0;


            _userId = user.UserId; //Guid.NewGuid().ToString().Replace("-","").ToLower();
            _userName = user.UserName;

            _userList = new BindingList<UserEntity>();
            _dicMsgList = new ConcurrentDictionary<string, BindingList<string>>();  // userId + Msgs

            // 添加系统用户，接受系统消息
            _curToId = _sysId = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            _userList.Add(new UserEntity { UserId = _sysId, UserName = "系统消息", DisplayName = "系统消息" });
            //初始化系统消息对象
            _dicMsgList[_sysId] = new BindingList<string>();

            // 获取聊天室频道
            GetChannelList();
            //初始化IM
            ConnWebSocket();

            //绑定数据
            _listBoxMsgs.DataSource = _dicMsgList[_sysId];
            _listBoxUser.DataSource = _userList;
            _listBoxUser.DisplayMember = "DisplayName";
            _listBoxUser.ValueMember = "UserId";

        }

        #endregion

    }
}
