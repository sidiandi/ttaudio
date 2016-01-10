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
            if (cancelConversion == null)
            {
                cancelConversion = new System.Threading.CancellationTokenSource();
            }

            Convert(cancelConversion.Token, files, textBoxTitle.Text);
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(About.GitUri.ToString());
        }

        private void exploreDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", AlbumMaker.GetDefaultDataDirectory().Quote());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void DeleteSelected()
        {
            var remainingItems = listViewInputFiles.Items.Cast<ListViewItem>().Where(_ => !_.Selected).ToArray();
            listViewInputFiles.Items.Clear();
            listViewInputFiles.Items.AddRange(remainingItems);
        }

        void SelectAll()
        {
            foreach (ListViewItem i in listViewInputFiles.Items)
            {
                i.Selected = true;
            }
        }

        private void listViewInputFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        SelectAll();
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        DeleteSelected();
                        break;
                }
            }
        }
    }
}
