using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using youtube2mp3.Properties;

namespace youtube2mp3
{
    public partial class MainForm : Form
    {
        private bool allowClose = false; // 默认不允许关闭

        private static string RootFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "youtube2mp3");

        private static string OutputFolder = Path.Combine(RootFolder, "output");

        private static string ffmpegPath = Path.Combine(RootFolder, "ffmpeg.exe");

        private static string ytdlpPath = Path.Combine(RootFolder, "yt-dlp.exe");

        private static string CookiePath = Path.Combine(RootFolder, "cookie.txt");

        private static string NewtonsoftPath = Path.Combine(RootFolder, "Newtonsoft.Json.dll");

        private static object locked = new object();

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                // 阻止用户点击“×”关闭
                e.Cancel = true;
                // 可选提示
                // MessageBox.Show("当前不能关闭窗口！");
            }

            base.OnFormClosing(e);
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

            LoadNewtonsoft();

            Kill();
            Directory.CreateDirectory(RootFolder);
            Directory.CreateDirectory(OutputFolder);
            CleanRootFolder();
            CleanOutputFolder();

            if (!File.Exists(ffmpegPath) || new FileInfo(ffmpegPath).Length == Resources.ffmpeg.Length)
            {
                File.WriteAllBytes(ffmpegPath, Resources.ffmpeg);
            }

            DownloadYTDLP(ytdlpPath);

            this.Invoke((MethodInvoker)(() =>
            {
                DownloadBtn.Text = "开始下载";
                DownloadBtn.Enabled = true;
                allowClose = true;
            }));
        }

        private static void LoadNewtonsoft()
        {
            if (!File.Exists(NewtonsoftPath))
                File.WriteAllBytes(NewtonsoftPath, Resources.Newtonsoft_Json);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(NewtonsoftPath);
        }

        private void DownloadYTDLP(string ytdlpPath)
        {
            try
            {
                string currentVersion = GetLocalYtDlpVersion();
                
                UpdateLatestVersion(currentVersion);
                currentVersion = GetLocalYtDlpVersion();
                if(currentVersion == null)
                {
                    throw new Exception("未知错误!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"无法下载yt-dlp，请检查网络！\n{ex.Message}");
                Environment.Exit(-1);
            }
        }

        private string GetLocalYtDlpVersion()
        {
            if (!File.Exists(ytdlpPath))
                return null;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ytdlpPath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    return output;
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error running yt-dlp: " + ex.Message);
                return null;
            }
        }

        private void UpdateLatestVersion(string currentVersion)
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
            client.Headers["Referer"] = "https://github.com/yt-dlp/yt-dlp/releases/";
            client.Headers["Oigin"] = "https://github.com";
            string result = client.DownloadString("https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest");
            JObject json = JObject.Parse(result);
            if (json.TryGetValue("tag_name", out JToken tagName))
            {
                if (currentVersion == null || !currentVersion.Equals(tagName.ToString()))
                {
                    string downloadUrl = $"https://github.com/yt-dlp/yt-dlp/releases/download/{tagName}/yt-dlp.exe";
                    var form = new DownloadYTDLPForm(downloadUrl);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        form.ShowDialog();
                        File.WriteAllBytes(ytdlpPath, form.FileData);
                        
                        form.Dispose();
                    }));
                }
            }
        }


        public static byte[] DecompressGZip(byte[] compressedData)
        {
            using (var compressedStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                gzipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
        private void Kill()
        {
            Process process = Process.GetCurrentProcess();
            foreach(var item in Process.GetProcesses())
            {
                try
                {
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
