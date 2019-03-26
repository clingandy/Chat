namespace ChatTestApp
{
    partial class ChatApp
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this._btnAddChannel = new System.Windows.Forms.Button();
            this._txtAddChannelName = new System.Windows.Forms.TextBox();
            this._listBoxChannel = new System.Windows.Forms.ListBox();
            this._btnRefreshChannel = new System.Windows.Forms.Button();
            this._btnOpenChannel = new System.Windows.Forms.Button();
            this._btnSendMsg = new System.Windows.Forms.Button();
            this._txtMsg = new System.Windows.Forms.TextBox();
            this._listBoxMsgs = new System.Windows.Forms.ListBox();
            this._listBoxUser = new System.Windows.Forms.ListBox();
            this._btnAddUser = new System.Windows.Forms.Button();
            this._txtAddUserName = new System.Windows.Forms.TextBox();
            this._tabControlChat = new System.Windows.Forms.TabControl();
            this._tabPageChat = new System.Windows.Forms.TabPage();
            this._tabPageChatroom = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._tabPageLogin = new System.Windows.Forms.TabPage();
            this._txtLoginName = new System.Windows.Forms.TextBox();
            this._btnLogin = new System.Windows.Forms.Button();
            this._lblLoginMsg = new System.Windows.Forms.Label();
            this._tabControlChat.SuspendLayout();
            this._tabPageChat.SuspendLayout();
            this._tabPageChatroom.SuspendLayout();
            this._tabPageLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnAddChannel
            // 
            this._btnAddChannel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnAddChannel.Location = new System.Drawing.Point(401, 74);
            this._btnAddChannel.Name = "_btnAddChannel";
            this._btnAddChannel.Size = new System.Drawing.Size(66, 23);
            this._btnAddChannel.TabIndex = 0;
            this._btnAddChannel.Text = "添加";
            this._btnAddChannel.UseVisualStyleBackColor = true;
            this._btnAddChannel.Click += new System.EventHandler(this._btnAddChannel_Click);
            // 
            // _txtAddChannelName
            // 
            this._txtAddChannelName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtAddChannelName.Location = new System.Drawing.Point(327, 35);
            this._txtAddChannelName.Name = "_txtAddChannelName";
            this._txtAddChannelName.Size = new System.Drawing.Size(215, 26);
            this._txtAddChannelName.TabIndex = 1;
            // 
            // _listBoxChannel
            // 
            this._listBoxChannel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._listBoxChannel.FormattingEnabled = true;
            this._listBoxChannel.ItemHeight = 14;
            this._listBoxChannel.Location = new System.Drawing.Point(325, 119);
            this._listBoxChannel.Name = "_listBoxChannel";
            this._listBoxChannel.Size = new System.Drawing.Size(217, 424);
            this._listBoxChannel.TabIndex = 4;
            this._listBoxChannel.DoubleClick += new System.EventHandler(this._listBoxChannel_DoubleClick);
            // 
            // _btnRefreshChannel
            // 
            this._btnRefreshChannel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnRefreshChannel.Location = new System.Drawing.Point(325, 74);
            this._btnRefreshChannel.Name = "_btnRefreshChannel";
            this._btnRefreshChannel.Size = new System.Drawing.Size(66, 23);
            this._btnRefreshChannel.TabIndex = 0;
            this._btnRefreshChannel.Text = "刷新";
            this._btnRefreshChannel.UseVisualStyleBackColor = true;
            this._btnRefreshChannel.Click += new System.EventHandler(this._btnRefreshChannel_Click);
            // 
            // _btnOpenChannel
            // 
            this._btnOpenChannel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnOpenChannel.Location = new System.Drawing.Point(478, 74);
            this._btnOpenChannel.Name = "_btnOpenChannel";
            this._btnOpenChannel.Size = new System.Drawing.Size(66, 23);
            this._btnOpenChannel.TabIndex = 0;
            this._btnOpenChannel.Text = "打开";
            this._btnOpenChannel.UseVisualStyleBackColor = true;
            this._btnOpenChannel.Click += new System.EventHandler(this._btnOpenChannel_Click);
            // 
            // _btnSendMsg
            // 
            this._btnSendMsg.Location = new System.Drawing.Point(668, 551);
            this._btnSendMsg.Name = "_btnSendMsg";
            this._btnSendMsg.Size = new System.Drawing.Size(102, 26);
            this._btnSendMsg.TabIndex = 5;
            this._btnSendMsg.Text = "发送消息";
            this._btnSendMsg.UseVisualStyleBackColor = true;
            this._btnSendMsg.Click += new System.EventHandler(this._btnSendMsg_Click);
            // 
            // _txtMsg
            // 
            this._txtMsg.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtMsg.Location = new System.Drawing.Point(213, 429);
            this._txtMsg.Multiline = true;
            this._txtMsg.Name = "_txtMsg";
            this._txtMsg.Size = new System.Drawing.Size(566, 157);
            this._txtMsg.TabIndex = 4;
            this._txtMsg.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._txtMsg_KeyPress);
            // 
            // _listBoxMsgs
            // 
            this._listBoxMsgs.FormattingEnabled = true;
            this._listBoxMsgs.HorizontalScrollbar = true;
            this._listBoxMsgs.ItemHeight = 12;
            this._listBoxMsgs.Location = new System.Drawing.Point(213, 9);
            this._listBoxMsgs.Name = "_listBoxMsgs";
            this._listBoxMsgs.Size = new System.Drawing.Size(566, 412);
            this._listBoxMsgs.TabIndex = 3;
            // 
            // _listBoxUser
            // 
            this._listBoxUser.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._listBoxUser.FormattingEnabled = true;
            this._listBoxUser.ItemHeight = 14;
            this._listBoxUser.Location = new System.Drawing.Point(6, 9);
            this._listBoxUser.Name = "_listBoxUser";
            this._listBoxUser.Size = new System.Drawing.Size(201, 508);
            this._listBoxUser.TabIndex = 2;
            this._listBoxUser.Click += new System.EventHandler(this._listBoxUser_Click);
            // 
            // _btnAddUser
            // 
            this._btnAddUser.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnAddUser.Location = new System.Drawing.Point(6, 556);
            this._btnAddUser.Name = "_btnAddUser";
            this._btnAddUser.Size = new System.Drawing.Size(201, 26);
            this._btnAddUser.TabIndex = 1;
            this._btnAddUser.Text = "添加好友";
            this._btnAddUser.UseVisualStyleBackColor = true;
            this._btnAddUser.Click += new System.EventHandler(this._btnAddUser_Click);
            // 
            // _txtAddUserName
            // 
            this._txtAddUserName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtAddUserName.Location = new System.Drawing.Point(6, 524);
            this._txtAddUserName.Name = "_txtAddUserName";
            this._txtAddUserName.Size = new System.Drawing.Size(201, 26);
            this._txtAddUserName.TabIndex = 0;
            this._txtAddUserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._txtAddUserName_KeyPress);
            // 
            // _tabControlChat
            // 
            this._tabControlChat.Controls.Add(this._tabPageChat);
            this._tabControlChat.Controls.Add(this._tabPageChatroom);
            this._tabControlChat.Controls.Add(this._tabPageLogin);
            this._tabControlChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControlChat.Location = new System.Drawing.Point(0, 0);
            this._tabControlChat.Name = "_tabControlChat";
            this._tabControlChat.SelectedIndex = 0;
            this._tabControlChat.Size = new System.Drawing.Size(795, 618);
            this._tabControlChat.TabIndex = 7;
            // 
            // _tabPageChat
            // 
            this._tabPageChat.Controls.Add(this._btnSendMsg);
            this._tabPageChat.Controls.Add(this._listBoxUser);
            this._tabPageChat.Controls.Add(this._txtMsg);
            this._tabPageChat.Controls.Add(this._txtAddUserName);
            this._tabPageChat.Controls.Add(this._listBoxMsgs);
            this._tabPageChat.Controls.Add(this._btnAddUser);
            this._tabPageChat.Location = new System.Drawing.Point(4, 22);
            this._tabPageChat.Name = "_tabPageChat";
            this._tabPageChat.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageChat.Size = new System.Drawing.Size(787, 592);
            this._tabPageChat.TabIndex = 0;
            this._tabPageChat.Text = "IM即时通信";
            this._tabPageChat.UseVisualStyleBackColor = true;
            // 
            // _tabPageChatroom
            // 
            this._tabPageChatroom.Controls.Add(this.label3);
            this._tabPageChatroom.Controls.Add(this.label2);
            this._tabPageChatroom.Controls.Add(this.label1);
            this._tabPageChatroom.Controls.Add(this._btnAddChannel);
            this._tabPageChatroom.Controls.Add(this._listBoxChannel);
            this._tabPageChatroom.Controls.Add(this._txtAddChannelName);
            this._tabPageChatroom.Controls.Add(this._btnOpenChannel);
            this._tabPageChatroom.Controls.Add(this._btnRefreshChannel);
            this._tabPageChatroom.Location = new System.Drawing.Point(4, 22);
            this._tabPageChatroom.Name = "_tabPageChatroom";
            this._tabPageChatroom.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageChatroom.Size = new System.Drawing.Size(787, 592);
            this._tabPageChatroom.TabIndex = 1;
            this._tabPageChatroom.Text = "聊天室";
            this._tabPageChatroom.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "聊天室列表：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "操作：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "聊天室名称：";
            // 
            // _tabPageLogin
            // 
            this._tabPageLogin.Controls.Add(this._lblLoginMsg);
            this._tabPageLogin.Controls.Add(this._txtLoginName);
            this._tabPageLogin.Controls.Add(this._btnLogin);
            this._tabPageLogin.Location = new System.Drawing.Point(4, 22);
            this._tabPageLogin.Name = "_tabPageLogin";
            this._tabPageLogin.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageLogin.Size = new System.Drawing.Size(787, 592);
            this._tabPageLogin.TabIndex = 2;
            this._tabPageLogin.Text = "登录";
            this._tabPageLogin.UseVisualStyleBackColor = true;
            // 
            // _txtLoginName
            // 
            this._txtLoginName.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtLoginName.Location = new System.Drawing.Point(296, 229);
            this._txtLoginName.Name = "_txtLoginName";
            this._txtLoginName.Size = new System.Drawing.Size(168, 30);
            this._txtLoginName.TabIndex = 1;
            this._txtLoginName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._txtLoginName_KeyPress);
            // 
            // _btnLogin
            // 
            this._btnLogin.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnLogin.Location = new System.Drawing.Point(296, 317);
            this._btnLogin.Name = "_btnLogin";
            this._btnLogin.Size = new System.Drawing.Size(168, 30);
            this._btnLogin.TabIndex = 0;
            this._btnLogin.Text = "登录";
            this._btnLogin.UseVisualStyleBackColor = true;
            this._btnLogin.Click += new System.EventHandler(this._btnLogin_Click);
            // 
            // _lblLoginMsg
            // 
            this._lblLoginMsg.AutoSize = true;
            this._lblLoginMsg.Location = new System.Drawing.Point(296, 267);
            this._lblLoginMsg.Name = "_lblLoginMsg";
            this._lblLoginMsg.Size = new System.Drawing.Size(0, 12);
            this._lblLoginMsg.TabIndex = 2;
            // 
            // ChatApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 618);
            this.Controls.Add(this._tabControlChat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ChatApp";
            this.ShowIcon = false;
            this.Text = "ChatApp";
            this._tabControlChat.ResumeLayout(false);
            this._tabPageChat.ResumeLayout(false);
            this._tabPageChat.PerformLayout();
            this._tabPageChatroom.ResumeLayout(false);
            this._tabPageChatroom.PerformLayout();
            this._tabPageLogin.ResumeLayout(false);
            this._tabPageLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _btnAddChannel;
        private System.Windows.Forms.TextBox _txtAddChannelName;
        private System.Windows.Forms.ListBox _listBoxChannel;
        private System.Windows.Forms.Button _btnRefreshChannel;
        private System.Windows.Forms.Button _btnOpenChannel;
        private System.Windows.Forms.Button _btnAddUser;
        private System.Windows.Forms.TextBox _txtAddUserName;
        private System.Windows.Forms.ListBox _listBoxUser;
        private System.Windows.Forms.ListBox _listBoxMsgs;
        private System.Windows.Forms.TextBox _txtMsg;
        private System.Windows.Forms.Button _btnSendMsg;
        private System.Windows.Forms.TabControl _tabControlChat;
        private System.Windows.Forms.TabPage _tabPageChat;
        private System.Windows.Forms.TabPage _tabPageChatroom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage _tabPageLogin;
        private System.Windows.Forms.TextBox _txtLoginName;
        private System.Windows.Forms.Button _btnLogin;
        private System.Windows.Forms.Label _lblLoginMsg;
    }
}

