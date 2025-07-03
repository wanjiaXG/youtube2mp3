using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace youtube2mp3
{
    public partial class DownloadYTDLPForm : Form
    {
        private string downloadUrl;
        private long contentLength;
        private int threadCount = 5;
        private byte[][] parts;
        private long totalBytesRead = 0;
        private int finishedThreads = 0;
        private object lockObj = new object();
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public byte[] FileData { get; private set; }
        public bool IsUserCancelled { get; private set; } = false;
        public bool IsFinish { get; private set; } = false;

        public DownloadYTDLPForm(string downloadUrl)
        {
            InitializeComponent();
            this.downloadUrl = downloadUrl;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!IsFinish && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "取消更新后本工具将无法使用，确定要退出吗？",
                    "提示",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                // 用户确认取消
                IsUserCancelled = true;
                cancelTokenSource.Cancel();

                // 设置 DialogResult 为 Cancel
                this.DialogResult = DialogResult.Cancel;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Thread t = new Thread(() => PrepareDownload(cancelTokenSource.Token));
            t.IsBackground = true;
            t.Start();
        }

        private void PrepareDownload(CancellationToken token)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(downloadUrl);
                req.Method = "HEAD";

                using (WebResponse resp = req.GetResponse())
                {
                    contentLength = resp.ContentLength;
                }

                if (contentLength <= 0)
                {
                    throw new Exception("网络错误！");
                }

                parts = new byte[threadCount][];
                long partSize = contentLength / threadCount;

                for (int i = 0; i < threadCount; i++)
                {
                    int index = i;
                    long start = index * partSize;
                    long end = (index == threadCount - 1) ? contentLength - 1 : (start + partSize - 1);

                    Thread t = new Thread(() => DownloadPart(index, start, end, token));
                    t.IsBackground = true;
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                // 可扩展：日志记录或弹窗提示
                Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show("下载准备失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                }));
            }
        }

        private void DownloadPart(int index, long start, long end, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested) return;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(downloadUrl);
                req.Method = "GET";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
                req.AddRange(start, end);

                using (WebResponse resp = req.GetResponse())
                using (Stream stream = resp.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while (!token.IsCancellationRequested &&
                           (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                        long totalSoFar = Interlocked.Add(ref totalBytesRead, bytesRead);
                        UpdateProgress((double)totalSoFar / contentLength * 100.0);
                    }

                    if (!token.IsCancellationRequested)
                    {
                        parts[index] = ms.ToArray();
                    }
                }
            }
            catch
            {
                // 可选：记录错误或显示警告
            }
            finally
            {
                if (!token.IsCancellationRequested) 
                {
                    lock (lockObj)
                    {
                        finishedThreads++;
                        if (finishedThreads == threadCount)
                        {
                            MergeParts();
                        }
                    }
                }
            }
        }

        private void UpdateProgress(double percent)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ProgressBar.Value = Math.Min(100, (int)percent);
                    ProgressLabel.Text = string.Format("{0:0.00}%", percent);
                });
            }
        }

        private void MergeParts()
        {
            MemoryStream output = new MemoryStream();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] != null)
                {
                    output.Write(parts[i], 0, parts[i].Length);
                }
            }

            FileData = output.ToArray();
            this.IsFinish = true;
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ProgressBar.Value = 100;
                    ProgressLabel.Text = "下载完成 100%";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                });
            }
        }
    }
}
