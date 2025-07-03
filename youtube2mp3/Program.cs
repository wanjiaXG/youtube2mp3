using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using youtube2mp3.Properties;

namespace youtube2mp3
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CheckSystemVersion();
            Application.Run(new MainForm());
        }

        private static void CheckSystemVersion()
        {
            var version = GetRealWindowsVersion();
            if (version != null && Version.TryParse(version, out Version ver))
            {
                // Windows 11 是从 Build 22000 起
                if (ver.Major == 10 && ver.Build >= 22000)
                {
                    if (!IsRunAsAdministrator())
                    {
                        DialogResult result = MessageBox.Show(
                            "检测到当前系统为Windows 11及以上，需要以管理员权限运行。\n是否重新启动？",
                            "权限提示",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                ProcessStartInfo psi = new ProcessStartInfo(Application.ExecutablePath)
                                {
                                    UseShellExecute = true,
                                    Verb = "runas"
                                };
                                Process.Start(psi);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"无法以管理员身份启动程序：\n{ex.Message}", "启动失败");
                            }

                            Environment.Exit(0);
                        }
                        else
                        {
                            new Thread(Exit).Start();
                            MessageBox.Show($"权限不足，程序稍后将自动关闭...");
                        }
                    }
                }
            }
        }

        private static void Exit()
        {
            Thread.Sleep(5000);
            Environment.Exit(-1);
        }

        private static string GetRealWindowsVersion()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Version FROM Win32_OperatingSystem"))
                {
                    foreach (var os in searcher.Get())
                    {
                        return os["Version"]?.ToString();
                    }
                }
            }
            catch { }
            return null;
        }

        private static bool IsRunAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}
