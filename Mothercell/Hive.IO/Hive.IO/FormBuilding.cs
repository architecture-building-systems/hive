using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ri = Rhino.Input.Custom;

namespace Hive.IO
{
    public partial class FormBuilding : Form
    {
        public Rhino.RhinoDoc RhDoc;
        public FormBuilding()
        {
            InitializeComponent();
        }

        public void SetRhinoDoc(Rhino.RhinoDoc doc)
        {
            this.RhDoc = doc;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            //ri.GetLine();
        }
    }
}
