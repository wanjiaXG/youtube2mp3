
namespace youtube2mp3
{
    partial class MainForm
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
            this.CookieLB = new System.Windows.Forms.Label();
            this.CookieTB = new System.Windows.Forms.TextBox();
            this.URLLB = new System.Windows.Forms.Label();
            this.URLTB = new System.Windows.Forms.TextBox();
            this.LogTB = new System.Windows.Forms.TextBox();
            this.DownloadBtn = new System.Windows.Forms.Button();
            this.OutputFolderBtn = new System.Windows.Forms.Button();
            this.BestVideoCB = new System.Windows.Forms.CheckBox();
            this.DownloadVideoCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CookieLB
            // 
            this.CookieLB.Location = new System.Drawing.Point(13, 13);
            this.CookieLB.Name = "CookieLB";
            this.CookieLB.Size = new System.Drawing.Size(49, 23);
            this.CookieLB.TabIndex = 0;
            this.CookieLB.Text = "Cookie";
            this.CookieLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CookieTB
            // 
            this.CookieTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CookieTB.Location = new System.Drawing.Point(15, 40);
            this.CookieTB.Multiline = true;
            this.CookieTB.Name = "CookieTB";
            this.CookieTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.CookieTB.Size = new System.Drawing.Size(779, 109);
            this.CookieTB.TabIndex = 1;
            // 
            // URLLB
            // 
            this.URLLB.Location = new System.Drawing.Point(13, 162);
            this.URLLB.Name = "URLLB";
            this.URLLB.Size = new System.Drawing.Size(60, 23);
            this.URLLB.TabIndex = 2;
            this.URLLB.Text = "视频地址";
            this.URLLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // URLTB
            // 
            this.URLTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.URLTB.Location = new System.Drawing.Point(69, 163);
            this.URLTB.Name = "URLTB";
            this.URLTB.Size = new System.Drawing.Size(643, 21);
            this.URLTB.TabIndex = 3;
            // 
            // LogTB
            // 
            this.LogTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTB.Location = new System.Drawing.Point(15, 202);
            this.LogTB.Multiline = true;
            this.LogTB.Name = "LogTB";
            this.LogTB.ReadOnly = true;
            this.LogTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogTB.Size = new System.Drawing.Size(779, 109);
            this.LogTB.TabIndex = 4;
            // 
            // DownloadBtn
            // 
            this.DownloadBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadBtn.Location = new System.Drawing.Point(719, 163);
            this.DownloadBtn.Name = "DownloadBtn";
            this.DownloadBtn.Size = new System.Drawing.Size(75, 23);
            this.DownloadBtn.TabIndex = 5;
            this.DownloadBtn.Text = "开始下载";
            this.DownloadBtn.UseVisualStyleBackColor = true;
            this.DownloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // OutputFolderBtn
            // 
            this.OutputFolderBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OutputFolderBtn.Location = new System.Drawing.Point(15, 317);
            this.OutputFolderBtn.Name = "OutputFolderBtn";
            this.OutputFolderBtn.Size = new System.Drawing.Size(147, 23);
            this.OutputFolderBtn.TabIndex = 6;
            this.OutputFolderBtn.Text = "打开输出文件夹";
            this.OutputFolderBtn.UseVisualStyleBackColor = true;
            this.OutputFolderBtn.Click += new System.EventHandler(this.OutputFolderBtn_Click);
            // 
            // BestVideoCB
            // 
            this.BestVideoCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BestVideoCB.Enabled = false;
            this.BestVideoCB.Location = new System.Drawing.Point(512, 318);
            this.BestVideoCB.Name = "BestVideoCB";
            this.BestVideoCB.Size = new System.Drawing.Size(282, 24);
            this.BestVideoCB.TabIndex = 7;
            this.BestVideoCB.Text = "最佳视频质量(耗时长，高CPU占用，不建议启用)";
            this.BestVideoCB.UseVisualStyleBackColor = true;
            // 
            // DownloadVideoCB
            // 
            this.DownloadVideoCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadVideoCB.Location = new System.Drawing.Point(419, 318);
            this.DownloadVideoCB.Name = "DownloadVideoCB";
            this.DownloadVideoCB.Size = new System.Drawing.Size(87, 24);
            this.DownloadVideoCB.TabIndex = 8;
            this.DownloadVideoCB.Text = "下载视频";
            this.DownloadVideoCB.UseVisualStyleBackColor = true;
            this.DownloadVideoCB.CheckedChanged += new System.EventHandler(this.AudioOnlyCB_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 351);
            this.Controls.Add(this.DownloadVideoCB);
            this.Controls.Add(this.BestVideoCB);
            this.Controls.Add(this.OutputFolderBtn);
            this.Controls.Add(this.DownloadBtn);
            this.Controls.Add(this.LogTB);
            this.Controls.Add(this.URLTB);
            this.Controls.Add(this.URLLB);
            this.Controls.Add(this.CookieTB);
            this.Controls.Add(this.CookieLB);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Youtube2MP3";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CookieLB;
        private System.Windows.Forms.TextBox CookieTB;
        private System.Windows.Forms.Label URLLB;
        private System.Windows.Forms.TextBox URLTB;
        private System.Windows.Forms.TextBox LogTB;
        private System.Windows.Forms.Button DownloadBtn;
        private System.Windows.Forms.Button OutputFolderBtn;
        private System.Windows.Forms.CheckBox BestVideoCB;
        private System.Windows.Forms.CheckBox DownloadVideoCB;
    }
}

