// Copyright (c) https://github.com/sidiandi 2016
// 
// This file is part of tta.
// 
// tta is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// tta is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

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
