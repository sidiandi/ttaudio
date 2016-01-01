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

            Trace.TraceInformation(p.StartInfo.Arguments);

            p.Start();

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                throw new Exception(p.StartInfo.Arguments);
            }
        }
    }
}
