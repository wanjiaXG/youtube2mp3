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


        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Directory.CreateDirectory(RootFolder);
            Directory.CreateDirectory(OutputFolder);
            CleanRootFolder();
            CleanOutputFolder();
            File.WriteAllBytes(ffmpegPath, Resources.ffmpeg);
            File.WriteAllBytes(ytdlpPath, Resources.yt_dlp);
        }

        private void CleanRootFolder()
        {

            foreach(var item in new DirectoryInfo(RootFolder).GetFiles("temp*"))
            {
                item.Delete();
            }
            File.Delete(CookiePath);
        }

        private void CleanOutputFolder()
        {
            foreach (var item in new DirectoryInfo(OutputFolder).GetFiles())
            {
                item.Delete();
            }

        }



        private void DownloadBtn_Click(object sender, EventArgs e)
        {
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

            URLTB.Enabled = CookieTB.Enabled = DownloadBtn.Enabled = false;
            new Thread(() =>
            {
                try
                {
                    if (Download(url))
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            MessageBox.Show($"下载成功，请自行使用spek等工具检测音频是否超编码");
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
                        URLTB.Enabled = CookieTB.Enabled = DownloadBtn.Enabled = true;
                        CleanRootFolder();
                        OutputFolderBtn.PerformClick();
                    }));
                }

            })
            { IsBackground = true}.Start();
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
            string tempfile = Path.Combine(RootFolder, "temp-file");

            string command = $"--cookies \"{CookiePath}\" -f bestaudio --extract-audio --audio-format best --audio-quality 0 -o \"{tempfile}\" \"{url}\"";

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

            string wavFile = Path.Combine(RootFolder, "temp.wav");
            command = $"-i \"{tempfile}\" \"{wavFile}\"";

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

            if (!File.Exists(wavFile)) return false;


            string mp3File = Path.Combine(OutputFolder, "output-192k.mp3");
            command = $"-i \"{wavFile}\" -b:a 192k \"{mp3File}\"";

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

            if (!File.Exists(mp3File)) return false;

            File.Copy(wavFile, Path.Combine(OutputFolder, "油管音源.wav"));
            return true;
        }
    }
}
