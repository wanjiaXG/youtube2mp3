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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using youtube2mp3.Properties;

namespace youtube2mp3
{
    public partial class MainForm : Form
    {
        private bool allowClose = false; // 默认不允许关闭

        public static string RootFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),  //"Local",
                "youtube2mp3");

        public static string OutputFolder = Path.Combine(RootFolder, "output");

        public static string ffmpegPath = Path.Combine(RootFolder, "ffmpeg.exe");

        public static string ytdlpPath = Path.Combine(RootFolder, "yt-dlp.exe");

        public static string CookiePath = Path.Combine(RootFolder, "cookie.txt");

        public static string NewtonsoftPath = Path.Combine(RootFolder, "Newtonsoft.Json.dll");

        public static string BassPath = Path.Combine(RootFolder, "bass.dll");

        public static string TimingAnlyzPath = Path.Combine(RootFolder, "TimingAnlyz.exe");

        public static string spekPath = Path.Combine(RootFolder, "spek.zip");

        public static string spekRootPath = Path.Combine(RootFolder, "spek");

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
                loadingLabel.Visible = true;
                //DownloadBtn.Text = "初始化中";
                DownloadBtn.Enabled = false;
            }));

            

            Kill();
            Directory.CreateDirectory(RootFolder);
            Directory.CreateDirectory(OutputFolder);
            CleanRootFolder();
            CleanOutputFolder();

            Extract(ffmpegPath, Resources.ffmpeg);
            Extract(BassPath, Resources.bass);
            Extract(TimingAnlyzPath, Resources.TimingAnlyz);
            
            LoadSpek();


            LoadNewtonsoft();
            DownloadYTDLP(ytdlpPath);
            
            this.Invoke((MethodInvoker)(() =>
            {
                loadingLabel.Visible = false;
                DownloadBtn.Enabled = true;
                allowClose = true;
            }));
        }

        private void LoadSpek()
        {
            Extract(spekPath, Resources.spek);
            Directory.CreateDirectory(spekRootPath);
            CleanFoler(spekRootPath);
            ZipFile.ExtractToDirectory(spekPath, spekRootPath);
        }


        private void Extract(string path, byte[] buff)
        {
            if (!File.Exists(path) || new FileInfo(path).Length == Resources.ffmpeg.Length)
            {
                File.WriteAllBytes(path, buff);
            }
        }

        private void LoadNewtonsoft()
        {
            Extract(NewtonsoftPath, Resources.Newtonsoft_Json);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
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
                    //var form = new DownloadYTDLPForm(downloadUrl);

                    this.Invoke((MethodInvoker)(() =>
                    {
                        using (var form = new DownloadYTDLPForm(downloadUrl))
                        {
                            var dialogResult = form.ShowDialog();
                            if (dialogResult == DialogResult.OK && !form.IsUserCancelled)
                            {
                                File.WriteAllBytes(ytdlpPath, form.FileData);
                            }
                            else
                            {
                                MessageBox.Show("下载失败或被中断！请检查网络后重试！");
                                Environment.Exit(-1);
                            }
                        }
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

            foreach (var item in Process.GetProcesses())
            {
                try
                {
                    if ((item.Id != process.Id && item.MainModule.FileName.Contains("youtube2mp3")) ||
                        (item.MainModule.FileName.Contains("spek") && item.MainModule.FileName.Contains("youtube2mp3")) ||
                        (item.MainModule.FileName.Contains("TimingAnlyz") && item.MainModule.FileName.Contains("youtube2mp3")) ||
                        (item.MainModule.FileName.Contains("ffmpeg") && item.MainModule.FileName.Contains("youtube2mp3")) ||
                        (item.MainModule.FileName.Contains("yt-dlp") && item.MainModule.FileName.Contains("youtube2mp3"))
                        )
                    {
                        item.Kill();
                    }
                }
                catch
                {

                }
            }
            
        }

        //private void CleanFoler(string path)
        //{
        //    for (int i = 0; i <= 10; i++)
        //    {
        //        try
        //        {
        //            foreach (var item in new DirectoryInfo(path).GetFiles())
        //            {
        //                item.Delete();
        //                break;
        //            }
        //        }
        //        catch
        //        {
        //            Thread.Sleep(1000);
        //        }
        //    }
        //}

        void CleanFoler(string path)
        {
            if (!Directory.Exists(path)) return;

            // 删除所有文件
            foreach (string file in Directory.GetFiles(path))
            {
                File.SetAttributes(file, FileAttributes.Normal); // 解除只读等属性
                File.Delete(file);
            }

            // 删除所有子目录
            foreach (string dir in Directory.GetDirectories(path))
            {
                CleanFoler(dir); // 递归删除子目录
                Directory.Delete(dir);
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
            CleanFoler(OutputFolder);
        }



        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            lock (locked)
            {
                if (DownloadBtn.Text.Equals("开始下载"))
                {
                    DownloadBtn.Enabled = false;
                    if(StartDownload() == 0)
                    {
                        DownloadBtn.Text = "取消下载";
                        DownloadBtn.Enabled = true;
                    }
                    else
                    {
                        DownloadBtn.Enabled = true;
                    }
                    
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

        public static bool IsValidUrl(string url)
        {
            string pattern = @"^https?://[^\s/$.?#].[^\s]*$";
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }

        private int StartDownload()
        {
            Kill();
            string cookie = CookieTB.Text;
            string url = URLTB.Text;
            string pattern = @"^(https?://)?([a-zA-Z0-9.-]+)(:[0-9]+)?(/.*)?$";


            if (!IsValidUrl(url))
            {
                MessageBox.Show("请输入正确的视频地址后再继续");
                return -1;
            }
/*
            if (url.Contains("youtube"))
            {
                //https://www.youtube.com/watch?v=1SWQWOs3HUA&list=PLrO67VV2uuoR2b2dYw_7jFXghMrInakS6
                int index = url.IndexOf("v=");
                if(index >= 0)
                {
                    string cut = url.Substring(index);
                    index = cut.IndexOf("&");
                    if(index >= 0)
                    {
                        url = $"https://www.youtube.com/watch?v={cut.Substring(2, index - 2)}";
                    }
                }
                

            }
            else if (url.Contains("youtu.be") || url.Contains("bilibili"))
            {
                int index = url.IndexOf("?");
                if (index >= 0)
                {
                    url = url.Substring(0, index);
                }
            }*/

            if ((url.Contains("youtu.be") || url.Contains("youtube.com")) && string.IsNullOrWhiteSpace(cookie))
            {
                MessageBox.Show("请输入cookie后再继续");
                return -1;
            }

            DisableControl();
            thread = new Thread(() =>
            {
                try
                {
                    string path = Download(url);
                    if (path != null)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            //MessageBox.Show($"下载成功，请自行使用spek等工具检测音频是否超编码");
                            var form = new MusicInfoForm();
                            form.ShowInfo(path);
                            form.Show();
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
            return 0;
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

        private string Download(string url)
        {
            CleanRootFolder();
            CleanOutputFolder();
            File.WriteAllText(CookiePath, CookieTB.Text);

            //Download video from youtube
            string tempfile = Path.Combine(RootFolder, "temp-file");


            string command =  $"{GetCookie(url)} {GetFormat()}--audio-format best --audio-quality 0 --no-playlist -o \"{tempfile}\" \"{url}\"";

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
            if (tempfile == null) return null;

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

            if (!File.Exists(audioFile)) return null;


            if (!DownloadVideoCB.Checked)
            {
                File.Copy(tempfile, Path.Combine(OutputFolder, $"[原始文件]{new FileInfo(tempfile).Name}"));
                return audioFile;
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

            if (!File.Exists(videoFile)) return null;

            File.Copy(tempfile, Path.Combine(OutputFolder, $"[原始文件]{new FileInfo(tempfile).Name}"));
            return audioFile;
        }

        private string GetCookie(string url)
        {
            //return $"--cookies \"{CookiePath}\"";
            if (url.Contains("youtube") || url.Contains("youtu.be")){
                return $"--cookies \"{CookiePath}\"";
            }
            return string.Empty;
        }

        private object GetFormat() => DownloadVideoCB.Checked ? "" : "-f bestaudio --extract-audio ";

        private object GetQuantity() => BestVideoCB.Checked ? "-preset placebo " : "";

        private void AudioOnlyCB_CheckedChanged(object sender, EventArgs e)
        {
            BestVideoCB.Enabled = DownloadVideoCB.Checked;
        }
    }
}
