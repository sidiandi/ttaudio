using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tta
{
    public partial class TaskForm : Form
    {
        Task task;
        CancellationTokenSource cancellationTokenSource;

        public TaskForm(Task task, CancellationTokenSource cancellationTokenSource)
        {
            this.task = task;
            // task.ContinueWith((t, o) => Close(), null, TaskScheduler.FromCurrentSynchronizationContext());
            this.cancellationTokenSource = cancellationTokenSource;
            InitializeComponent();
            this.FormClosing += TaskForm_FormClosing;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var a = new TextboxAppender(this.textBoxProgress)
            {
                Layout = new PatternLayout("%utcdate{ISO8601} %level %message%newline"),
                Threshold = Level.Info,
                Name = textBoxProgress.Name,
            };
            a.ActivateOptions();
            appender = a;

            ((Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(appender);
        }

        log4net.Appender.IAppender appender;

        private void TaskForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!task.IsCompleted)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    Cancel();
                }
                e.Cancel = true;
                return;
            }

            appender.Close();
            ((Hierarchy)log4net.LogManager.GetRepository()).Root.RemoveAppender(appender);
        }

        void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
