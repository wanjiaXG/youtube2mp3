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

        public byte[] FileData { get; private set; }

        public DownloadYTDLPForm(string downloadUrl)
        {
            InitializeComponent();
            this.downloadUrl = downloadUrl;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Thread t = new Thread(PrepareDownload);
            t.IsBackground = true;
            t.Start();
        }


        private void PrepareDownload()
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
                    ShowError("服务器未返回有效的文件大小");
                    return;
                }

                parts = new byte[threadCount][];
                long partSize = contentLength / threadCount;

                for (int i = 0; i < threadCount; i++)
                {
                    int index = i;
                    long start = index * partSize;
                    long end = (index == threadCount - 1) ? contentLength - 1 : (start + partSize - 1);

                    Thread t = new Thread(() => DownloadPart(index, start, end));
                    t.IsBackground = true;
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                ShowError("初始化失败：" + ex.Message);
            }
        }

        private void DownloadPart(int index, long start, long end)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(downloadUrl);
                req.Method = "GET";
                req.UserAgent = "Mozilla/5.0";
                req.AddRange(start, end);

                using (WebResponse resp = req.GetResponse())
                using (Stream stream = resp.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                        long totalSoFar = Interlocked.Add(ref totalBytesRead, bytesRead);
                        UpdateProgress((double)totalSoFar / contentLength * 100.0);
                    }

                    parts[index] = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                ShowError("线程 " + index + " 下载失败：" + ex.Message);
            }
            finally
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

        private void UpdateProgress(double percent)
        {
            this.Invoke((MethodInvoker)delegate
            {
                ProgressBar.Value = Math.Min(100, (int)percent);
                ProgressLabel.Text = string.Format("{0:0.00}%", percent);
            });
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

            this.Invoke((MethodInvoker)delegate
            {
                ProgressBar.Value = 100;
                ProgressLabel.Text = "下载完成 100%";
                this.DialogResult = DialogResult.OK;  // 设置返回结果
                this.Close();
            });
        }

        private void ShowError(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            });
        }
    }
}
