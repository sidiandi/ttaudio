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
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using log4net.Layout;
using log4net.Core;
using RavSoft;
using ttaenc;

namespace ttaudio
{
    public partial class Editor : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        readonly Document document;

        public Editor(Document document)
        {
            if (document == null)
            {
                document = new Document { package = new Package() };
            }

            this.document = document;

            InitializeComponent();

            CueProvider.SetCue(textBoxTitle, "automatic");
            CueProvider.SetCue(textBoxProductId, "automatic");

            UpdateView();
        }

        private void listViewInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Add(files);
            }
        }

        /// <summary>
        /// Add input files to the list view
        /// </summary>
        /// <param name="inputFiles"></param>
        public void Add(IEnumerable<string> inputFiles)
        {
            foreach (var audioFile in new AlbumReader().GetAudioFiles(inputFiles))
            {
                this.listViewInputFiles.Items.Add(new ListViewItem(audioFile)
                {
                    Tag = audioFile
                });
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

        Task Convert(CancellationToken cancel, IList<string> files, string title, string productId)
        {
            return Task.Factory.StartNew(async () => 
            {
                try
                {
                    var package = Package.CreateFromInputPaths(files);
                    
                    if (!String.IsNullOrEmpty(title))
                    {
                        package.Name = title;
                    }
                    if (!String.IsNullOrEmpty(productId))
                    {
                        package.ProductId = UInt16.Parse(productId);
                    }

                    var converter = Context.GetDefaultMediaFileConverter();

                    var pen = TipToiPen.GetAll().First();
                    var packageBuilder = new PackageBuilder(
                        new PackageDirectoryStructure(pen.RootDirectory, package), converter);

                    await packageBuilder.Build(cancel);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }, cancel);
        }

        void StartConversion()
        {
            UpdateModel();

            var files = listViewInputFiles.Items.Cast<ListViewItem>().Select(_ => (string)_.Tag).ToList();
            if (!files.Any())
            {
                MessageBox.Show("Drop some audio files into the list first.");
                return;
            }

            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            var task = Convert(cancellationTokenSource.Token, files, textBoxTitle.Text, textBoxProductId.Text);

            var taskForm = new TaskForm(task, cancellationTokenSource)
            {
                Text = "Convert and Copy to Pen"
            };
                
            taskForm.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(About.GitUri.ToString());
        }

        private void exploreDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Process.Start("explorer.exe", AlbumMaker.GetDefaultDataDirectory().Quote());
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        void Save()
        {
            UpdateModel();

            if (document.ttaFile == null)
            {
                SaveAs();
                return;
            }

            document.Save();
        }

        const string fileDialogFilter = "TipToi Game Files (*.gme)|*.gme";

        void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = About.DocumentsDirectory;
            saveFileDialog.Filter = Document.fileDialogFilter;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                document.ttaFile = saveFileDialog.FileName;
            }
            Save();
        }

        void UpdateView()
        {
            var p = document.package;

            listViewInputFiles.Items.Clear();
            Add(document.package.Albums.SelectMany(_ => _.Tracks.Select(track => track.Path)));

            this.textBoxTitle.Text = p.Name;
            if (p.ProductId != 0)
            {
                this.textBoxProductId.Text = p.ProductId.ToString();
            }

            this.Text = this.document.ttaFile;
        }

        void UpdateModel()
        {
            var files = listViewInputFiles.Items.Cast<ListViewItem>().Select(_ => (string)_.Tag).ToList();
            var p = Package.CreateFromInputPaths(files);
            p.Name = this.textBoxTitle.Text;
            int productId;
            if (Int32.TryParse(this.textBoxProductId.Text, out productId))
            {
                p.ProductId = productId;
            }
            document.package = p;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print();
        }

        void Print()
        {
            UpdateModel();
            Save();
            var builder = GetPackageBuilder();
            var cts = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => builder.OpenHtmlPage(cts.Token), TaskCreationOptions.LongRunning);
            var f = new TaskForm(task, cts);
            f.Show();
        }

        void Upload()
        {
            UpdateModel();
            Save();
            var builder = GetPackageBuilder();
            var cts = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => builder.Build(cts.Token), TaskCreationOptions.LongRunning);
            var f = new TaskForm(task, cts);
            f.Show();
        }

        PackageBuilder GetPackageBuilder()
        {
            var s = new PackageDirectoryStructure(GetRootDirectory(), this.document.package);
            return new PackageBuilder(s, Context.GetDefaultMediaFileConverter());
        }

        public static string GetRootDirectory()
        {
            return TipToiPen.Get().RootDirectory;
        }

        private void uploadToPenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Upload();
        }
    }
}
