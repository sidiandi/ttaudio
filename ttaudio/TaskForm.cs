using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ttaenc;

namespace ttaudio
{
    public partial class TaskForm : Form
    {
        Task task;
        CancellationTokenSource cancellationTokenSource;

        public static Task StartTask(string caption, System.Action action)
        {
            var cts = new CancellationTokenSource();
            var task = Task.Factory.StartNew(action, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            var tw = new TaskForm(task, cts) { Text = caption };
            tw.Show();
            return task;
        }

        public TaskForm(Task task, CancellationTokenSource cancellationTokenSource)
        {
            InitializeComponent();

            this.task = task;
            stopwatch = Stopwatch.StartNew();
            timer.Start();
            task.ContinueWith((t, o) => OnComplete(), null, TaskScheduler.FromCurrentSynchronizationContext());
            this.cancellationTokenSource = cancellationTokenSource;
            this.FormClosing += TaskForm_FormClosing;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelResult.Text = String.Format("Running for {0:F0} seconds", stopwatch.Elapsed.TotalSeconds);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var patternLayout = new PatternLayout();
            patternLayout.AddConverter("rfc3339", typeof(Rfc3339Converter));
            patternLayout.ConversionPattern = "%rfc3339 %level %message%newline";
            patternLayout.ActivateOptions();

            var a = new TextboxAppender(this.textBoxProgress)
            {
                Layout = patternLayout,
                Threshold = Level.Debug,
                Name = textBoxProgress.Name,
            };
            a.ActivateOptions();
            appender = a;

            ((Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(appender);
        }

        Stopwatch stopwatch;

        void OnComplete()
        {
            timer.Stop();
            if (task.IsFaulted)
            {
                labelResult.Text = String.Format("Task failed. See log for details.");
            }
            else
            {
                labelResult.Text = String.Format("Task completed sucessfully.");
                buttonCancel.Text = "&Close";
                this.Close();
            }
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
