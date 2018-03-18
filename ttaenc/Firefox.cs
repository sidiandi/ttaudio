using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class Firefox
    {
        public static string Executable => Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramW6432%"), @"Mozilla Firefox\firefox.exe");

        public static bool IsInstalled => File.Exists(Executable);

        public static void OpenHtmlPage(string htmlPage)
        {
            Process.Start(Executable, htmlPage);
        }
    }
}
