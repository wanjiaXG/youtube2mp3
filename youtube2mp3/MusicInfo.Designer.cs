
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
            this.SuspendLayout();
            // 
            // SpekPanel
            // 
            this.SpekPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SpekPanel.Location = new System.Drawing.Point(12, 240);
            this.SpekPanel.Name = "SpekPanel";
            this.SpekPanel.Size = new System.Drawing.Size(776, 393);
            this.SpekPanel.TabIndex = 0;
            // 
            // TimingPanel
            // 
            this.TimingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TimingPanel.Location = new System.Drawing.Point(12, 12);
            this.TimingPanel.Name = "TimingPanel";
            this.TimingPanel.Size = new System.Drawing.Size(776, 222);
            this.TimingPanel.TabIndex = 1;
            // 
            // MusicInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 645);
            this.Controls.Add(this.TimingPanel);
            this.Controls.Add(this.SpekPanel);
            this.Name = "MusicInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MusicInfo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SpekPanel;
        private System.Windows.Forms.Panel TimingPanel;
    }
}