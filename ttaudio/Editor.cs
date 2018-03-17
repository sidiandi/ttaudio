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
        About about = About.Get();

        public Editor(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            this.document = document;

            InitializeComponent();

            this.listViewInputFiles.Columns.Clear();
            var mWidth = (int) listViewInputFiles.Font.Size;
            this.listViewInputFiles.Columns.AddRange(new[] {
                    new ColumnHeader { Text = "Oid", Width = 5 * mWidth },
                    new ColumnHeader { Text = "Artist", Width = 16 * mWidth },
                    new ColumnHeader { Text = "Album", Width = 16 * mWidth},
                    new ColumnHeader { Text = "#", Width = 5 * mWidth},
                    new ColumnHeader { Text = "Title", Width = 32 * mWidth},
            });

            CueProvider.SetCue(textBoxTitle, "automatic");
            CueProvider.SetCue(textBoxProductId, "automatic");

            UpdateView();
        }

        public static Editor Open(string file)
        {
            var e = new Editor(Document.Load(file));
            e.Show();
            return e;
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
        public async Task Add(IEnumerable<string> inputFiles)
        {
            UpdateModel();
            await TaskForm.StartTask("Add Files", () => this.document.package.AddTracks(inputFiles));
            UpdateView();
        }

        private void listViewInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(about.GithubUri.ToString());
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
            document.package.RemoveTracks(listViewInputFiles.Items.Cast<ListViewItem>().Where(_ => _.Selected)
                .Select(_ => (Track)_.Tag));
            UpdateView();
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
            if (document.ttaFile == null)
            {
                SaveAs();
                return;
            }

            UpdateModel();
            document.Save();
        }

        const string fileDialogFilter = "TipToi Game Files (*.gme)|*.gme";

        void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = about.DocumentsDirectory;
            saveFileDialog.Filter = Document.fileDialogFilter;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                document.ttaFile = saveFileDialog.FileName;
                Save();
                UpdateView();
            }
        }

        void UpdateView()
        {
            Package p;
            lock (this)
            {
                p = document.package;
            }

            listViewInputFiles.Items.Clear();
            listViewInputFiles.Items.AddRange(p.Tracks.Select(track => new ListViewItem(new string[] {
                track.Oid.ToString(),
                String.Join(", ", track.Artists),
                track.Album,
                track.TrackNumber.ToString(),
                track.Title
            })
            {
                Tag = track
            }).ToArray());

            this.textBoxTitle.Text = p.Title;
            if (p.ProductId != 0)
            {
                this.textBoxProductId.Text = p.ProductId.ToString();
            }

            this.comboBoxPlaybackMode.SelectedIndex = (int) p.PlaybackMode;

            this.Text = String.Join(" - ", new string[] { about.Product, this.document.ttaFile }
                .Where(_ => !String.IsNullOrEmpty(_)));
        }

        void UpdateModel()
        {
            lock (this)
            {
                if (this.InvokeRequired)
                {
                    throw new InvalidOperationException("Must be called from UI thread.");
                }

                var p = document.package;
                p.Title = this.textBoxTitle.Text;
                int productId;
                if (Int32.TryParse(this.textBoxProductId.Text, out productId))
                {
                    p.ProductId = productId;
                }
                p.PlaybackMode = (PlaybackModes)this.comboBoxPlaybackMode.SelectedIndex;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print();
        }

        public void Print()
        {
            UpdateModel();
            var builder = GetPackageBuilder();
            var cts = new CancellationTokenSource();
            var task = Task.Factory.StartNew(async () =>
            {
                await builder.CreateHtmlPage(cts.Token);
                await builder.OpenHtmlPage(cts.Token);
            }, TaskCreationOptions.LongRunning);
            var f = new TaskForm(task, cts) { Text = "Print" };
            f.Show();
        }

        public void Upload()
        {
            UpdateModel();
            var builder = GetPackageBuilder();
            var cts = new CancellationTokenSource();

            var task = Task.Factory.StartNew(async () =>
            {
                await builder.Build(cts.Token);

                var pen = TipToiPen.GetAll().FirstOrDefault();
                if (pen != null)
                {
                    await builder.Upload(cts.Token, pen);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format("Upload is not possible because the Tiptoi pen is not connected."), about.Product);
                    ShowOutput(builder);
                }

                log.Info("complete");
            }, TaskCreationOptions.LongRunning);

            var f = new TaskForm(task, cts) { Text = "Convert and Upload" };
            f.Show();
        }

        IProductIdProvider productIdProvider = new ProductIdProvider();

        PackageBuilder GetPackageBuilder()
        {
            // fill ProductId if empty
            if (this.document.package.ProductId == 0)
            {
                this.document.package.ProductId = productIdProvider.GetNextAvailableProductId();
                UpdateView();
            }

            var s = new PackageDirectoryStructure(Path.Combine(about.DocumentsDirectory, "output"), this.document.package);
            return new PackageBuilder(s, Context.GetDefaultMediaFileConverter(), Settings.Read().CreateOidSvgWriter());
        }

        public static string GetRootDirectory()
        {
            return TipToiPen.Get().RootDirectory;
        }

        private void uploadToPenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Upload();
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ShowAboutInformation();
        }

        public void ShowAboutInformation()
        {
            Process.Start(about.GithubUri.ToString());
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstanceOpen();
        }

        public void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = about.DocumentsDirectory,
            };
            PathUtil.EnsureDirectoryExists(openFileDialog.InitialDirectory);

            openFileDialog.Filter = Document.fileDialogFilter;
            if (openFileDialog.ShowDialog(null) == DialogResult.OK)
            {
                Open(openFileDialog.FileName);
            }
        }

        public void InstanceOpen()
        {
            Open();
            CloseIfEmpty();
        }

        public void InstanceNew()
        {
            New();
            CloseIfEmpty();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstanceNew();
        }

        void CloseIfEmpty()
        {
            if (!document.package.Tracks.Any())
            {
                Close();
            }
        }

        public static Editor New()
        {
            var e = new Editor(new Document());
            e.Show();
            return e;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void printerTestPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var testPage = Path.Combine(about.LocalApplicationDataDirectory, "tiptoi-printer-test.html");
                PathUtil.EnsureParentDirectoryExists(testPage);
                OidSvgWriter.CreatePrinterTestPage(testPage);
                Process.Start(testPage);
            }, TaskCreationOptions.LongRunning);
        }

        private void printToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Print();
        }

        private void uploadToPenToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Upload();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Upload();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void Editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var optionsDialog = new Options();
            var s = ttaenc.Settings.Read();
            optionsDialog.GridSpacing = s.OidGridSpacing;
            optionsDialog.DotSize = s.OidDotSize;
            optionsDialog.UpdateView();
            if (optionsDialog.ShowDialog() == DialogResult.OK)
            {
                optionsDialog.UpdateModel();
                s.OidGridSpacing = (float) optionsDialog.GridSpacing;
                s.OidDotSize = (float) optionsDialog.DotSize;
                s.Write();
            }
        }

        static void ShowOutput(PackageBuilder builder)
        {
            Process.Start("explorer.exe", "/select," + builder.packageDirectoryStructure.GmeFile.Quote());
        }

        private void showOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateModel();
            var builder = GetPackageBuilder();
            ShowOutput(builder);
        }

        private void assignOIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new AssignOids();
            dlg.FirstOid = 10250;
            dlg.UpdateView();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                dlg.UpdateModel();
                this.document.package.AssignOids(dlg.FirstOid);
                UpdateView();
            }
        }
    }
}
