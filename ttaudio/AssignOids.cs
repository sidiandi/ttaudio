using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ttaudio
{
    public partial class AssignOids : Form
    {
        public AssignOids()
        {
            InitializeComponent();
        }

        public int FirstOid;

        public void UpdateView()
        {
            textBoxFirstOid.Text = FirstOid.ToString();
        }

        public void UpdateModel()
        {
            FirstOid = Int32.Parse(textBoxFirstOid.Text);
        }
    }
}
