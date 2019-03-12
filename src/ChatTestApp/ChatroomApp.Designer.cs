namespace PubSubTestApp
{
    partial class ChatroomApp
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
            this.SuspendLayout();
            // 
            // _btnAddChannel
            // 
            this._btnAddChannel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnAddChannel.Location = new System.Drawing.Point(104, 454);
            this._btnAddChannel.Name = "_btnAddChannel";
            this._btnAddChannel.Size = new System.Drawing.Size(64, 23);
            this._btnAddChannel.TabIndex = 0;
            this._btnAddChannel.Text = "添加";
            this._btnAddChannel.UseVisualStyleBackColor = true;
            this._btnAddChannel.Click += new System.EventHandler(this._btnAddChannel_Click);
            // 
            // _txtAddChannelName
            // 
            this._txtAddChannelName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtAddChannelName.Location = new System.Drawing.Point(12, 412);
            this._txtAddChannelName.Name = "_txtAddChannelName";
            this._txtAddChannelName.Size = new System.Drawing.Size(251, 26);
            this._txtAddChannelName.TabIndex = 1;
            // 
            // _listBoxChannel
            // 
            this._listBoxChannel.FormattingEnabled = true;
            this._listBoxChannel.ItemHeight = 12;
            this._listBoxChannel.Location = new System.Drawing.Point(12, 12);
            this._listBoxChannel.Name = "_listBoxChannel";
            this._listBoxChannel.Size = new System.Drawing.Size(251, 388);
            this._listBoxChannel.TabIndex = 4;
            this._listBoxChannel.DoubleClick += new System.EventHandler(this._listBoxChannel_DoubleClick);
            // 
            // _btnRefreshChannel
            // 
            this._btnRefreshChannel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnRefreshChannel.Location = new System.Drawing.Point(12, 454);
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
            this._btnOpenChannel.Location = new System.Drawing.Point(199, 454);
            this._btnOpenChannel.Name = "_btnOpenChannel";
            this._btnOpenChannel.Size = new System.Drawing.Size(64, 23);
            this._btnOpenChannel.TabIndex = 0;
            this._btnOpenChannel.Text = "打开";
            this._btnOpenChannel.UseVisualStyleBackColor = true;
            this._btnOpenChannel.Click += new System.EventHandler(this._btnOpenChannel_Click);
            // 
            // ChatroomApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 489);
            this.Controls.Add(this._listBoxChannel);
            this.Controls.Add(this._txtAddChannelName);
            this.Controls.Add(this._btnRefreshChannel);
            this.Controls.Add(this._btnOpenChannel);
            this.Controls.Add(this._btnAddChannel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChatroomApp";
            this.ShowIcon = false;
            this.Text = "ChatroomApp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnAddChannel;
        private System.Windows.Forms.TextBox _txtAddChannelName;
        private System.Windows.Forms.ListBox _listBoxChannel;
        private System.Windows.Forms.Button _btnRefreshChannel;
        private System.Windows.Forms.Button _btnOpenChannel;
    }
}

