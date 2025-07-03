
namespace youtube2mp3
{
    partial class InfoUserControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SpekPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // SpekPanel
            // 
            this.SpekPanel.Location = new System.Drawing.Point(0, 0);
            this.SpekPanel.Name = "SpekPanel";
            this.SpekPanel.Size = new System.Drawing.Size(865, 401);
            this.SpekPanel.TabIndex = 0;
            // 
            // InfoUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SpekPanel);
            this.Name = "InfoUserControl";
            this.Size = new System.Drawing.Size(865, 640);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SpekPanel;
    }
}
