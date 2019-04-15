namespace ChatTestApp
{
    partial class Chatroom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._listBoxMsg = new System.Windows.Forms.ListBox();
            this._btnSendMsg = new System.Windows.Forms.Button();
            this._txtMsg = new System.Windows.Forms.TextBox();
            this._listBoxUserList = new System.Windows.Forms.ListBox();
            this._btnAddThread = new System.Windows.Forms.Button();
            this._btnSendMsgTest = new System.Windows.Forms.Button();
            this._txtThreadSleepTime = new System.Windows.Forms.TextBox();
            this._btnClearMsg = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._txtAddThreadCount = new System.Windows.Forms.TextBox();
            this._txtSendMsgCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._btnStopSendMsgTest = new System.Windows.Forms.Button();
            this._btnShowReceiveInfo = new System.Windows.Forms.Button();
            this._listBoxReceiveInfo = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _listBoxMsg
            // 
            this._listBoxMsg.FormattingEnabled = true;
            this._listBoxMsg.HorizontalScrollbar = true;
            this._listBoxMsg.ItemHeight = 12;
            this._listBoxMsg.Location = new System.Drawing.Point(260, 13);
            this._listBoxMsg.Name = "_listBoxMsg";
            this._listBoxMsg.Size = new System.Drawing.Size(542, 424);
            this._listBoxMsg.TabIndex = 10;
            // 
            // _btnSendMsg
            // 
            this._btnSendMsg.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnSendMsg.Location = new System.Drawing.Point(679, 550);
            this._btnSendMsg.Name = "_btnSendMsg";
            this._btnSendMsg.Size = new System.Drawing.Size(114, 29);
            this._btnSendMsg.TabIndex = 8;
            this._btnSendMsg.Text = "发送消息";
            this._btnSendMsg.UseVisualStyleBackColor = true;
            this._btnSendMsg.Click += new System.EventHandler(this._btnSendMsg_Click);
            // 
            // _txtMsg
            // 
            this._txtMsg.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtMsg.Location = new System.Drawing.Point(260, 443);
            this._txtMsg.MaxLength = 450;
            this._txtMsg.Multiline = true;
            this._txtMsg.Name = "_txtMsg";
            this._txtMsg.Size = new System.Drawing.Size(542, 149);
            this._txtMsg.TabIndex = 6;
            this._txtMsg.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._txtMsg_KeyPress);
            // 
            // _listBoxUserList
            // 
            this._listBoxUserList.FormattingEnabled = true;
            this._listBoxUserList.ItemHeight = 12;
            this._listBoxUserList.Location = new System.Drawing.Point(13, 13);
            this._listBoxUserList.Name = "_listBoxUserList";
            this._listBoxUserList.Size = new System.Drawing.Size(241, 76);
            this._listBoxUserList.TabIndex = 11;
            // 
            // _btnAddThread
            // 
            this._btnAddThread.Location = new System.Drawing.Point(36, 379);
            this._btnAddThread.Name = "_btnAddThread";
            this._btnAddThread.Size = new System.Drawing.Size(187, 29);
            this._btnAddThread.TabIndex = 12;
            this._btnAddThread.Text = "开始增加连接数";
            this._btnAddThread.UseVisualStyleBackColor = true;
            this._btnAddThread.Click += new System.EventHandler(this._btnAddThread_Click);
            // 
            // _btnSendMsgTest
            // 
            this._btnSendMsgTest.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnSendMsgTest.Location = new System.Drawing.Point(112, 481);
            this._btnSendMsgTest.Name = "_btnSendMsgTest";
            this._btnSendMsgTest.Size = new System.Drawing.Size(111, 29);
            this._btnSendMsgTest.TabIndex = 13;
            this._btnSendMsgTest.Text = "并发消息测试";
            this._btnSendMsgTest.UseVisualStyleBackColor = true;
            this._btnSendMsgTest.Click += new System.EventHandler(this._btnSendMsgTest_Click);
            // 
            // _txtThreadSleepTime
            // 
            this._txtThreadSleepTime.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtThreadSleepTime.Location = new System.Drawing.Point(160, 449);
            this._txtThreadSleepTime.Name = "_txtThreadSleepTime";
            this._txtThreadSleepTime.Size = new System.Drawing.Size(63, 26);
            this._txtThreadSleepTime.TabIndex = 14;
            this._txtThreadSleepTime.Text = "500";
            this._txtThreadSleepTime.Leave += new System.EventHandler(this._txtThreadSleepTime_Leave);
            // 
            // _btnClearMsg
            // 
            this._btnClearMsg.Location = new System.Drawing.Point(36, 516);
            this._btnClearMsg.Name = "_btnClearMsg";
            this._btnClearMsg.Size = new System.Drawing.Size(187, 29);
            this._btnClearMsg.TabIndex = 15;
            this._btnClearMsg.Text = "清理消息列表";
            this._btnClearMsg.UseVisualStyleBackColor = true;
            this._btnClearMsg.Click += new System.EventHandler(this._btnClearMsg_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(34, 456);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "并发消息间隔：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(34, 348);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 16;
            this.label2.Text = "添加并发数量：";
            // 
            // _txtAddThreadCount
            // 
            this._txtAddThreadCount.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtAddThreadCount.Location = new System.Drawing.Point(160, 345);
            this._txtAddThreadCount.Name = "_txtAddThreadCount";
            this._txtAddThreadCount.Size = new System.Drawing.Size(63, 26);
            this._txtAddThreadCount.TabIndex = 14;
            this._txtAddThreadCount.Text = "1000";
            this._txtAddThreadCount.Leave += new System.EventHandler(this._txtThreadSleepTime_Leave);
            // 
            // _txtSendMsgCount
            // 
            this._txtSendMsgCount.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtSendMsgCount.Location = new System.Drawing.Point(160, 417);
            this._txtSendMsgCount.Name = "_txtSendMsgCount";
            this._txtSendMsgCount.Size = new System.Drawing.Size(63, 26);
            this._txtSendMsgCount.TabIndex = 14;
            this._txtSendMsgCount.Text = "10";
            this._txtSendMsgCount.Leave += new System.EventHandler(this._txtThreadSleepTime_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(34, 424);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "并发消息数量：";
            // 
            // _btnStopSendMsgTest
            // 
            this._btnStopSendMsgTest.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnStopSendMsgTest.Location = new System.Drawing.Point(37, 481);
            this._btnStopSendMsgTest.Name = "_btnStopSendMsgTest";
            this._btnStopSendMsgTest.Size = new System.Drawing.Size(69, 29);
            this._btnStopSendMsgTest.TabIndex = 13;
            this._btnStopSendMsgTest.Text = "停止发送";
            this._btnStopSendMsgTest.UseVisualStyleBackColor = true;
            this._btnStopSendMsgTest.Click += new System.EventHandler(this._btnStopSendMsgTest_Click);
            // 
            // _btnShowReceiveInfo
            // 
            this._btnShowReceiveInfo.Location = new System.Drawing.Point(37, 551);
            this._btnShowReceiveInfo.Name = "_btnShowReceiveInfo";
            this._btnShowReceiveInfo.Size = new System.Drawing.Size(187, 29);
            this._btnShowReceiveInfo.TabIndex = 15;
            this._btnShowReceiveInfo.Text = "显示接受消息情况";
            this._btnShowReceiveInfo.UseVisualStyleBackColor = true;
            this._btnShowReceiveInfo.Click += new System.EventHandler(this._btnShowReceiveInfo_Click);
            // 
            // _listBoxReceiveInfo
            // 
            this._listBoxReceiveInfo.FormattingEnabled = true;
            this._listBoxReceiveInfo.HorizontalScrollbar = true;
            this._listBoxReceiveInfo.ItemHeight = 12;
            this._listBoxReceiveInfo.Location = new System.Drawing.Point(13, 95);
            this._listBoxReceiveInfo.Name = "_listBoxReceiveInfo";
            this._listBoxReceiveInfo.Size = new System.Drawing.Size(241, 244);
            this._listBoxReceiveInfo.TabIndex = 11;
            // 
            // Chatroom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 604);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._btnShowReceiveInfo);
            this.Controls.Add(this._btnClearMsg);
            this.Controls.Add(this._txtAddThreadCount);
            this.Controls.Add(this._txtSendMsgCount);
            this.Controls.Add(this._txtThreadSleepTime);
            this.Controls.Add(this._btnStopSendMsgTest);
            this.Controls.Add(this._btnSendMsgTest);
            this.Controls.Add(this._btnAddThread);
            this.Controls.Add(this._btnSendMsg);
            this.Controls.Add(this._listBoxReceiveInfo);
            this.Controls.Add(this._listBoxUserList);
            this.Controls.Add(this._listBoxMsg);
            this.Controls.Add(this._txtMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Chatroom";
            this.ShowIcon = false;
            this.Text = "Chatroom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chatroom_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Chatroom_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox _listBoxMsg;
        private System.Windows.Forms.Button _btnSendMsg;
        private System.Windows.Forms.TextBox _txtMsg;
        private System.Windows.Forms.ListBox _listBoxUserList;
        private System.Windows.Forms.Button _btnAddThread;
        private System.Windows.Forms.Button _btnSendMsgTest;
        private System.Windows.Forms.TextBox _txtThreadSleepTime;
        private System.Windows.Forms.Button _btnClearMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _txtAddThreadCount;
        private System.Windows.Forms.TextBox _txtSendMsgCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _btnStopSendMsgTest;
        private System.Windows.Forms.Button _btnShowReceiveInfo;
        private System.Windows.Forms.ListBox _listBoxReceiveInfo;
    }
}