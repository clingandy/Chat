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
            this._listBoxUserList.Size = new System.Drawing.Size(241, 580);
            this._listBoxUserList.TabIndex = 11;
            // 
            // _btnAddThread
            // 
            this._btnAddThread.Location = new System.Drawing.Point(503, 550);
            this._btnAddThread.Name = "_btnAddThread";
            this._btnAddThread.Size = new System.Drawing.Size(150, 29);
            this._btnAddThread.TabIndex = 12;
            this._btnAddThread.Text = "开始增加连接数";
            this._btnAddThread.UseVisualStyleBackColor = true;
            this._btnAddThread.Click += new System.EventHandler(this._btnAddThread_Click);
            // 
            // _btnSendMsgTest
            // 
            this._btnSendMsgTest.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnSendMsgTest.Location = new System.Drawing.Point(376, 550);
            this._btnSendMsgTest.Name = "_btnSendMsgTest";
            this._btnSendMsgTest.Size = new System.Drawing.Size(121, 29);
            this._btnSendMsgTest.TabIndex = 13;
            this._btnSendMsgTest.Text = "并发消息测试";
            this._btnSendMsgTest.UseVisualStyleBackColor = true;
            this._btnSendMsgTest.Click += new System.EventHandler(this._btnSendMsgTest_Click);
            // 
            // _txtThreadSleepTime
            // 
            this._txtThreadSleepTime.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtThreadSleepTime.Location = new System.Drawing.Point(270, 550);
            this._txtThreadSleepTime.Name = "_txtThreadSleepTime";
            this._txtThreadSleepTime.Size = new System.Drawing.Size(100, 26);
            this._txtThreadSleepTime.TabIndex = 14;
            this._txtThreadSleepTime.Text = "500";
            this._txtThreadSleepTime.Leave += new System.EventHandler(this._txtThreadSleepTime_Leave);
            // 
            // Chatroom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 604);
            this.Controls.Add(this._txtThreadSleepTime);
            this.Controls.Add(this._btnSendMsgTest);
            this.Controls.Add(this._btnAddThread);
            this.Controls.Add(this._btnSendMsg);
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
    }
}