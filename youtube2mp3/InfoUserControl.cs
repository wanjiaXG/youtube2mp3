using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace youtube2mp3
{
    public partial class InfoUserControl : UserControl
    {
        const int SpekWindowsOffset = 55;

        const int GWL_STYLE = -16;
        const int WS_BORDER = 0x00800000;
        const int WS_CAPTION = 0x00C00000;
        const int WS_THICKFRAME = 0x00040000;
        const int WS_MINIMIZEBOX = 0x00020000;
        const int WS_MAXIMIZEBOX = 0x00010000;
        const int WS_SYSMENU = 0x00080000;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_FRAMECHANGED = 0x0020;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public string SpekPath;
        public InfoUserControl()
        {
            SpekPath = Path.Combine(MainForm.spekRootPath, "spek.exe");
            InitializeComponent();
        }

        private Process _process;
        private IntPtr _windowHandle;

        public void KillSpek()
        {
            if (_process != null)
            {
                _process.Kill();
                _process = null;
                _windowHandle = IntPtr.Zero;
            }
        }

        public void ShowSpek(string FilePath = "")
        {
            KillSpek();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = SpekPath,
                Arguments = FilePath
            };

            _process = Process.Start(info);
            _process.WaitForInputIdle();
            _windowHandle = _process.MainWindowHandle;

            IntPtr style = GetWindowLong(_windowHandle, GWL_STYLE);
            IntPtr newStyle = new IntPtr(style.ToInt64() & ~(WS_BORDER | WS_CAPTION | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_SYSMENU));

            SetWindowLong(_windowHandle, GWL_STYLE, newStyle);
            SetWindowPos(_windowHandle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
            SetParent(_windowHandle, SpekPanel.Handle);
            MoveWindow(_windowHandle, 0, -SpekWindowsOffset, SpekPanel.Width, SpekPanel.Height + SpekWindowsOffset, true);
        }
        private void SpekPanel_SizeChanged(object sender, EventArgs e)
        {
            if (_windowHandle != IntPtr.Zero) MoveWindow(_windowHandle, 0, -SpekWindowsOffset, SpekPanel.Width, SpekPanel.Height + SpekWindowsOffset, true);
        }
    }
}



