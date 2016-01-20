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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Windows.Forms;

namespace ttaudio
{
    class TextboxAppender : log4net.Appender.AppenderSkeleton
    {
        public TextboxAppender(TextBox textBox)
        {
            this.textBox = textBox;
        }

        TextBox textBox;

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (this)
            {
                if (textBox == null)
                {
                    return;
                }

                var msg = this.RenderLoggingEvent(loggingEvent);
                if (textBox.InvokeRequired)
                {
                    textBox.BeginInvoke(new MethodInvoker(() => textBox.AppendText(msg)));
                }
            }
        }

        protected override void OnClose()
        {
            lock (this)
            {
                textBox = null;
            }
            base.OnClose();
        }
    }
}
