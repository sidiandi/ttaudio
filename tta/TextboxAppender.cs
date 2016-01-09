using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Windows.Forms;

namespace ttab
{
    class TextboxAppender : log4net.Appender.AppenderSkeleton
    {
        public TextboxAppender(TextBox textBox)
        {
            this.textBox = textBox;
        }

        readonly TextBox textBox;

        protected override void Append(LoggingEvent loggingEvent)
        {
            var msg = this.RenderLoggingEvent(loggingEvent);
            if (textBox.IsHandleCreated)
            {
                textBox.Invoke(new MethodInvoker(() => textBox.AppendText(msg)));
            }
        }
    }
}
