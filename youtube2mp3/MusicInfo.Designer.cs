
namespace youtube2mp3
{
    partial class MusicInfoForm
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
            this.SpekPanel = new System.Windows.Forms.Panel();
            this.TimingPanel = new System.Windows.Forms.Panel();
            this.TimingLabel = new System.Windows.Forms.Label();
            this.SpekLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SpekPanel
            // 
            this.SpekPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SpekPanel.Location = new System.Drawing.Point(12, 273);
            this.SpekPanel.Name = "SpekPanel";
            this.SpekPanel.Size = new System.Drawing.Size(776, 480);
            this.SpekPanel.TabIndex = 0;
            // 
            // TimingPanel
            // 
            this.TimingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TimingPanel.Location = new System.Drawing.Point(12, 37);
            this.TimingPanel.Name = "TimingPanel";
            this.TimingPanel.Size = new System.Drawing.Size(776, 207);
            this.TimingPanel.TabIndex = 1;
            // 
            // TimingLabel
            // 
            this.TimingLabel.Location = new System.Drawing.Point(12, 9);
            this.TimingLabel.Name = "TimingLabel";
            this.TimingLabel.Size = new System.Drawing.Size(776, 23);
            this.TimingLabel.TabIndex = 3;
            this.TimingLabel.Text = "Timing(仅匀速曲子有效，BPM相对正确，Offset需要校对):";
            this.TimingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SpekLabel
            // 
            this.SpekLabel.Location = new System.Drawing.Point(10, 247);
            this.SpekLabel.Name = "SpekLabel";
            this.SpekLabel.Size = new System.Drawing.Size(776, 23);
            this.SpekLabel.TabIndex = 4;
            this.SpekLabel.Text = "Spek(音频质量检测):";
            this.SpekLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MusicInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 765);
            this.Controls.Add(this.SpekLabel);
            this.Controls.Add(this.TimingLabel);
            this.Controls.Add(this.TimingPanel);
            this.Controls.Add(this.SpekPanel);
            this.Name = "MusicInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "音频信息";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SpekPanel;
        private System.Windows.Forms.Panel TimingPanel;
        private System.Windows.Forms.Label TimingLabel;
        private System.Windows.Forms.Label SpekLabel;
    }
}