using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawingMaster
{
    public partial class Form3 : Form
    {
        public Panel panel;
        public Form3()
        {
            InitializeComponent();
            panel = panel1;
            panel1.Visible = false;
            this.TopMost = true;
            TransparencyKey = BackColor;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            panel.Width = Width - SystemInformation.BorderSize.Width;
            panel.Height = Height - SystemInformation.CaptionHeight;
        }
    }
}
