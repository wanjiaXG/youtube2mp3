using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static youtube2mp3.ExternalProcessEmbedder;

namespace youtube2mp3
{
    public partial class MusicInfoForm : Form
    {
        private string AudioPath { set; get; } 
        public MusicInfoForm()
        {
            InitializeComponent();
        }

        public void ShowInfo(string audioPath)
        {
            AudioPath = audioPath;
            string SpekPath = Path.Combine(MainForm.spekRootPath, "spek.exe");

            var spek = new ExternalProcessEmbedder();
            spek.EmbedProcess(SpekPath, AudioPath, SpekPanel);

            var timing = new ExternalProcessEmbedder();
            timing.EmbedProcess(MainForm.TimingAnlyzPath, AudioPath, TimingPanel);
            if (timing.embeddedProcess != null && !timing.embeddedProcess.HasExited)
            {
                SetForegroundWindow(timing.embeddedProcess.MainWindowHandle);
                SendKeys.SendWait("{ENTER}");
            }
        }
    }


    public class ExternalProcessEmbedder
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // WinAPI 函数声明
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool repaint);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const int VK_RETURN = 0x0D;

        // 发送一次回车按键消息
        

        const int GWL_STYLE = -16;
        const int WS_CHILD = 0x40000000;
        const int WS_BORDER = 0x00800000;
        const int WS_DLGFRAME = 0x00400000;
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;

        public Process embeddedProcess;
        private IntPtr embeddedWindowHandle;


        public void EmbedProcess(string exePath, string arguments, Control hostControl)
        {
            // 启动外部程序
            embeddedProcess = new Process();
            embeddedProcess.StartInfo.FileName = $"\"{exePath}\"";
            embeddedProcess.StartInfo.Arguments = $"\"{arguments}\"";
            embeddedProcess.StartInfo.UseShellExecute = false;
            embeddedProcess.Start();

            // 等待主窗口句柄可用（轮询）
            int waitTime = 0;
            const int waitMax = 5000; // 5秒超时
            while (embeddedProcess.MainWindowHandle == IntPtr.Zero && waitTime < waitMax)
            {
                Thread.Sleep(100);
                embeddedProcess.Refresh();
                waitTime += 100;
            }
            embeddedWindowHandle = embeddedProcess.MainWindowHandle;

            if (embeddedWindowHandle == IntPtr.Zero)
                throw new Exception("无法获取外部程序主窗口句柄");

            // 设置父窗口为hostControl
            SetParent(embeddedWindowHandle, hostControl.Handle);

            // 修改窗口样式，去掉边框和标题栏，设置为子窗口
            int style = GetWindowLong(embeddedWindowHandle, GWL_STYLE);
            style = (style | WS_CHILD) & ~WS_CAPTION;
            SetWindowLong(embeddedWindowHandle, GWL_STYLE, style);

            // 调整大小使其填充 hostControl
            MoveWindow(embeddedWindowHandle, 0, 0, hostControl.Width, hostControl.Height, true);

            // 监听 hostControl 尺寸变化，调整外部窗口大小
            hostControl.Resize += (s, e) =>
            {
                if (embeddedWindowHandle != IntPtr.Zero)
                    MoveWindow(embeddedWindowHandle, 0, 0, hostControl.Width, hostControl.Height, true);
            };
        }

        public void CloseEmbeddedProcess()
        {
            try
            {
                if (embeddedProcess != null && !embeddedProcess.HasExited)
                {
                    embeddedProcess.Kill();
                    embeddedProcess.Dispose();
                }
            }
            catch { }
            embeddedProcess = null;
            embeddedWindowHandle = IntPtr.Zero;
        }
    }
}
