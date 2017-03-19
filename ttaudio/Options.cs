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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        // unit: cm
        public double DotSize;

        // unit: cm
        public double GridSpacing;

        const double displayLengthUnit = 1e-4;

        public void UpdateView()
        {
            textBoxDotSize.Text = (DotSize / displayLengthUnit).ToString("F0");
            textBoxGridSpacing.Text = (GridSpacing / displayLengthUnit).ToString("F0");
        }

        public void UpdateModel()
        {
            DotSize = Double.Parse(textBoxDotSize.Text) * displayLengthUnit;
            GridSpacing = Double.Parse(textBoxGridSpacing.Text) * displayLengthUnit;
        }
    }
}
