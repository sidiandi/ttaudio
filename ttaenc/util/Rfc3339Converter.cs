using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class Rfc3339Converter : log4net.Util.PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            var e = (log4net.Core.LoggingEvent)state;
            writer.Write(e.TimeStamp.ToInternetDateTimeFormat());
        }
    }
}
