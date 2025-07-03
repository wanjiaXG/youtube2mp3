using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            Application.Run(new MainForm());
            //MusicInfoForm form = new MusicInfoForm();
            //form.ShowInfo(@"C:\Users\admin\Documents\youtube2mp3\output-audio-192k.mp3");
            //Application.Run(form);
        }

        
    }
}
