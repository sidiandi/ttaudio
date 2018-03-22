using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sidi
{
    public partial class About : Form
    {
        readonly ttaenc.About about;
        public About(ttaenc.About about)
        {
            this.about = about;
            InitializeComponent();

            this.textBoxAboutInfo.Text =
@"" + about.Product + @" " + about.Version;

            this.Text = about.Product;
            linkLabelHomepage.Text = about.GithubUri.ToString();
            linkLabelHomepage.Click += LinkLabelHomepage_Click;
        }

        private void LinkLabelHomepage_Click(object sender, EventArgs e)
        {
            Process.Start(about.GithubUri.ToString());
        }
    }
}
