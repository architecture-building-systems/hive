using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hive.IO
{
    public partial class FormEnSysPV : Form
    {
        public List<double> Efficiency { get; private set; }
        public List<string> Technology { get; private set; }
        public List<System.Drawing.Bitmap> Image { get; private set; }
        public List<string> HelperText { get; private set; }

        public FormEnSysPV()
        {
            InitializeComponent();

            Technology = new List<string>();
            Efficiency = new List<double>();
            Image = new List<Bitmap>();
            HelperText = new List<string>();

            foreach (string tech in this.comboBox1.Items)
                Technology.Add(tech);

            Image.Add(global::Hive.IO.Properties.Resources.article_18);
            Image.Add(global::Hive.IO.Properties.Resources.fraunhofer);
            Image.Add(global::Hive.IO.Properties.Resources.asf);
            Image.Add(global::Hive.IO.Properties.Resources.stardestroyer);

            Efficiency.Add(0.1);
            Efficiency.Add(0.2);
            Efficiency.Add(0.3);
            Efficiency.Add(0.99);

            HelperText.Add("Mono-cristalline PV is like super old, boring");
            HelperText.Add("Breakthrough technology, Fraunhofer have reached a new milestone");
            HelperText.Add("A/S shows everyone how to do it. innovations everywhere");
            HelperText.Add("Execute order 66");
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.materialarchiv.ch/app-tablet/");
        }
    }
}
