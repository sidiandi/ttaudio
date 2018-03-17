using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            this.Text = about.Product;
            labelProduct.Text = about.Info;
            linkLabelHomepage.Text = about.GithubUri.ToString();
        }

    }
}
