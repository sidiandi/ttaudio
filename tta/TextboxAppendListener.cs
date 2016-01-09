using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tta
{
    class TextboxAppendListener : TraceListener
    {
        public TextboxAppendListener(TextBox textBox)
        {
            this.textBox = textBox;
        }

        readonly TextBox textBox;

        public override void Write(string message)
        {
            textBox.Invoke(new MethodInvoker(() => textBox.AppendText(message)));
        }

        public override void WriteLine(string message)
        {
            textBox.Invoke(new MethodInvoker(() => textBox.AppendText(message + "\r\n")));
        }
    }
}
