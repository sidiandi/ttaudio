using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaudio
{
    static class TaskExtensions
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Forget(this Task task)
        {
            task.ContinueWith(
                t => { log.Error(t.Exception); },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
