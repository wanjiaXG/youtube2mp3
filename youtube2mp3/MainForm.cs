using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using youtube2mp3.Properties;

namespace youtube2mp3
{
    public partial class MainForm : Form
    {
        private static string RootFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "youtube2mp3");

        private static string OutputFolder = Path.Combine(RootFolder, "output");

        private static string ffmpegPath = Path.Combine(RootFolder, "ffmpeg.exe");

        private static string ytdlpPath = Path.Combine(RootFolder, "yt-dlp.exe");

        private static string CookiePath = Path.Combine(RootFolder, "cookie.txt");

        private static object locked = new object();

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            new Thread(Init).Start();
        }

        private void Init()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                DownloadBtn.Text = "初始化中";
                DownloadBtn.Enabled = false;
            }));

            Kill();
            Directory.CreateDirectory(RootFolder);
            Directory.CreateDirectory(OutputFolder);
            CleanRootFolder();
            CleanOutputFolder();
            File.WriteAllBytes(ffmpegPath, Resources.ffmpeg);
            File.WriteAllBytes(ytdlpPath, Resources.yt_dlp);

            this.Invoke((MethodInvoker)(() =>
            {
                DownloadBtn.Text = "开始下载";
                DownloadBtn.Enabled = true;
            }));
        }

        private void Kill()
        {
            Process process = Process.GetCurrentProcess();
            foreach(var item in Process.GetProcesses())
            {
                try
                {
                    Console.WriteLine(item.MainModule.FileName);
                    if(item.Id != process.Id && item.MainModule.FileName.Contains("youtube2mp3"))
                    {
                        item.Kill();
                    }
                }
                catch
                {

                }
            }
        }

        private void CleanRootFolder()
        {
            for (int i = 0; i <= 10; i++)
            {
                try
                {
                    foreach (var item in new DirectoryInfo(RootFolder).GetFiles("temp*"))
                    {
                        item.Delete();
                    }
                    break;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            File.Delete(CookiePath);
        }

        private void CleanOutputFolder()
        {
            for(int i = 0; i <= 10; i++)
            {
                try
                {
                    foreach (var item in new DirectoryInfo(OutputFolder).GetFiles())
                    {
                        item.Delete();
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }



        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            lock (locked)
            {
                if (DownloadBtn.Text.Equals("开始下载"))
                {
                    DownloadBtn.Enabled = false;
                    StartDownload();
                    DownloadBtn.Text = "取消下载";
                    DownloadBtn.Enabled = true;
                }
                else if(DownloadBtn.Text.Equals("取消下载"))
                {
                    StopDownload();
                }
            }

            
        }

        private Thread thread;

        private void StopDownload()
        {
            try
            {
                thread.Abort();
            }
            catch
            {

            }
            finally
            {
                EnabledControl();
            }
        }

        private void StartDownload()
        {
            Kill();
            string cookie = CookieTB.Text;
            string url = URLTB.Text;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                MessageBox.Show("请输入cookie后再继续");
                return;
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("请输入视频地址后再继续");
                return;
            }

            DisableControl();
            thread = new Thread(() =>
            {
                try
                {
                    if (Download(url))
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            MessageBox.Show($"下载成功，请自行使用spek等工具检测音频是否超编码");
                            OutputFolderBtn.PerformClick();
                        }));

                    }
                    else
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            MessageBox.Show($"下载失败");
                        }));
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show($"下载失败！{ex.Message}");
                    }));
                }
                finally
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        EnabledControl();
                    }));
                }

            })
            { IsBackground = true };
            
            thread.Start();
        }

        private void DisableControl()
        {
            URLTB.Enabled = CookieTB.Enabled = DownloadVideoCB.Enabled = BestVideoCB.Enabled = false;
        }

        private void EnabledControl()
        {
            URLTB.Enabled = CookieTB.Enabled = DownloadVideoCB.Enabled = true;
            BestVideoCB.Enabled = DownloadVideoCB.Checked;
            DownloadBtn.Text = "开始下载";
            CleanRootFolder();
        }

        private void OutputFolderBtn_Click(object sender, EventArgs e)
        {
            Process.Start(OutputFolder);
        }

        private bool Download(string url)
        {
            CleanRootFolder();
            CleanOutputFolder();
            File.WriteAllText(CookiePath, CookieTB.Text);

            //Download video from youtube
            string tempfile = Path.Combine(RootFolder, "temp-file");
            string command = $"--cookies \"{CookiePath}\" {GetFormat()}--audio-format best --audio-quality 0 -o \"{tempfile}\" \"{url}\"";

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = ytdlpPath;
            info.Arguments = command;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;

            using (Process proc = Process.Start(info))
            {
                int maxCount = 10;
                int count = 0;
                string tmp;
                while ((tmp = proc.StandardOutput.ReadLine()) != null || count > maxCount)
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        count++;
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            LogTB.AppendText(tmp);
                            LogTB.AppendText("\r\n");
                        }));
                    }
                }
            }

            tempfile = null;
            foreach (var item in new DirectoryInfo(RootFolder).GetFiles("temp-*"))
            {
                tempfile = item.FullName;
                break;
            }
            if (tempfile == null) return false;

            //Convert audio file.
            string audioFile = Path.Combine(OutputFolder, "output-audio-192k.mp3");
            command = $"-i \"{tempfile}\" -b:a 192k \"{audioFile}\"";

            info = new ProcessStartInfo();
            info.FileName = ffmpegPath;
            info.Arguments = command;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;

            using (Process proc = Process.Start(info))
            {
                int maxCount = 10;
                int count = 0;
                string tmp;
                while ((tmp = proc.StandardError.ReadLine()) != null || count > maxCount)
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        count++;
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            LogTB.AppendText(tmp);
                            LogTB.AppendText("\r\n");
                        }));
                    }
                }
            }

            if (!File.Exists(audioFile)) return false;


            if (!DownloadVideoCB.Checked)
            {
                File.Copy(tempfile, Path.Combine(OutputFolder, $"[原始文件]{new FileInfo(tempfile).Name}"));
                return true;
            }

            //Convert video file.
            string videoFile = Path.Combine(OutputFolder, "output-video-720p-1200k.avi");
            command = $"-i \"{tempfile}\" {GetQuantity()}-vf \"scale=-1:720\" -an -b:v 1200k -vcodec libx264 \"{videoFile}\"";

            info = new ProcessStartInfo();
            info.FileName = ffmpegPath;
            info.Arguments = command;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;

            using (Process proc = Process.Start(info))
            {
                int maxCount = 10;
                int count = 0;
                string tmp;
                while ((tmp = proc.StandardError.ReadLine()) != null || count > maxCount)
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        count++;
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            LogTB.AppendText(tmp);
                            LogTB.AppendText("\r\n");
                        }));
                    }
                }
            }

            if (!File.Exists(videoFile)) return false;

            File.Copy(tempfile, Path.Combine(OutputFolder, $"[原始文件]{new FileInfo(tempfile).Name}"));
            return true;
        }

        private object GetFormat() => DownloadVideoCB.Checked ? "" : "-f bestaudio --extract-audio ";

        private object GetQuantity() => BestVideoCB.Checked ? "-preset placebo " : "";

        private void AudioOnlyCB_CheckedChanged(object sender, EventArgs e)
        {
            BestVideoCB.Enabled = DownloadVideoCB.Checked;
        }
    }
}
