using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ttab;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using log4net.Layout;
using log4net.Core;

namespace tta
{
    public partial class MainForm : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainForm()
        {
            InitializeComponent();

            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;

            ConfigureLogging();

            New();
        }

        void ConfigureLogging()
        {
            var a = new TextboxAppender(textBoxLog)
            {
                Layout = new PatternLayout("%utcdate{ISO8601} %level %message%newline"),
                Threshold = Level.Info,
                Name = textBoxLog.Name,
            };
            a.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(a);
        }

        private void listViewInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Add(AlbumReader.GetAudioFiles(files));
            }
        }

        /// <summary>
        /// Add input files to the list view
        /// </summary>
        /// <param name="inputFiles"></param>
        void Add(IEnumerable<string> inputFiles)
        {
            foreach (var inputFile in inputFiles)
            {
                this.listViewInputFiles.Items.Add(new ListViewItem(inputFile) { Tag = inputFile });
            }
            this.listViewInputFiles.Columns[this.listViewInputFiles.Columns.Count - 1].Width = -1;
        }

        private void listViewInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            StartConversion();
        }

        Task conversion;
        System.Threading.CancellationTokenSource cancelConversion;

        Task Convert(CancellationToken cancel, IList<string> files, string title)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var dataDirectory = AlbumMaker.GetDefaultDataDirectory();
                    var albumMaker = new AlbumMaker(dataDirectory);
                    var collection = new AlbumReader().FromTags(files);
                    if (!String.IsNullOrEmpty(title))
                    {
                        collection.Title = title;
                    }
                    albumMaker.Create(cancelConversion.Token, collection);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }, cancel);
        }

        void StartConversion()
        {
            var files = listViewInputFiles.Items.Cast<ListViewItem>().Select(_ => (string)_.Tag).ToList();
            if (!files.Any())
            {
                MessageBox.Show("Drop some audio files into the list first.");
                return;
            }

            textBoxLog.Clear();
            tabControl.SelectedTab = tabPageConversion;
            cancelConversion = new System.Threading.CancellationTokenSource();
            conversion = Convert(cancelConversion.Token, files, textBoxTitle.Text);
        }

        private void buttonStartNewConversion_Click(object sender, EventArgs e)
        {
            New();
        }

        void New()
        {
            this.tabControl.SelectedTab = this.tabPageInput;
            textBoxTitle.Text = String.Empty;
            this.listViewInputFiles.Items.Clear();
        }

        void CancelConversion()
        {
            if (cancelConversion != null)
            {
                cancelConversion.Cancel();
                cancelConversion = null;
            }
        }
    }
}
