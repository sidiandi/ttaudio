using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttab
{
    class SubProcess
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Cmd(string command)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = System.Environment.GetEnvironmentVariable("COMSPEC"),
                    Arguments = "/c " + command,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false
                }
            };

            log.Debug(Details(p));

            p.Start();

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                throw new Exception(String.Format("Exit code {0}: {1}", p.ExitCode, Details(p)));
            }
        }

        public static string Details(Process p)
        {
            return String.Format("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
        }
    }
}
