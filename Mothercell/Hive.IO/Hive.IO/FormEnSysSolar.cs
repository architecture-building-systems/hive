using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Hive.IO.Properties;
using System.IO;
using System.Reflection;

namespace Hive.IO
{
    public partial class FormEnSysSolar : Form
    {
        public string SystemType { get; private set; }
        public List<double> ElectricEfficiency { get; private set; }
        public List<double> ThermalEfficiency { get; private set; }
        public List<string> Technology { get; private set; }
        public List<double> Cost { get; private set; }
        public List<double> CO2 { get; private set; }

        public List<System.Drawing.Bitmap> Image { get; private set; }
        public List<string> HelperText { get; private set; }




        public FormEnSysSolar()
        {
            InitializeComponent();

            radioButton1.Checked = true;
            SystemType = "pv";
            LoadData("pv");
        }

        public void LoadData(string tech)
        {
            Technology = new List<string>();
            ElectricEfficiency = new List<double>();
            ThermalEfficiency = new List<double>();
            Cost = new List<double>();
            CO2 = new List<double>();
            Image = new List<Bitmap>();
            HelperText = new List<string>();
            string database;

            switch (tech)
            {
                case "pv":
                default:
                    // image names could be the same as the technology names (Technology[index]). And if there is no match, use empty image
                    // currently, this has to be manually changed if the database.csv changes
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.monocristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.monocristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.CIGS);
                    Image.Add(global::Hive.IO.Properties.Resources.CIGS);
                    Image.Add(global::Hive.IO.Properties.Resources.CdTe);
                    Image.Add(global::Hive.IO.Properties.Resources.CdTe);
                    Image.Add(global::Hive.IO.Properties.Resources.HIT);
                    Image.Add(global::Hive.IO.Properties.Resources.HIT);

                    // helper text could also be added into the database csv
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Monocristalline");
                    HelperText.Add("Monocristalline");
                    HelperText.Add("CIGS");
                    HelperText.Add("CIGS");
                    HelperText.Add("CdTe");
                    HelperText.Add("CdTe");
                    HelperText.Add("HIT");
                    HelperText.Add("HIT");

                    database = Resources.pv_efficiency;
                    SystemType = "pv";
                    break;
                case "st":
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    database = Resources.st_efficiency;
                    SystemType = "st";
                    break;
                case "pvt":
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    database = Resources.pvt_efficiency;
                    SystemType = "pvt";
                    break;
                case "gc":
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    Image.Add(global::Hive.IO.Properties.Resources.polycristalline);
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    HelperText.Add("Polycristalline");
                    database = Resources.gc_efficiency;
                    SystemType = "gc";
                    break;
            }




            string[] splitString = database.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < splitString.Length; i++)
            {
                string[] values = splitString[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                Technology.Add(values[0]);
                ElectricEfficiency.Add(Convert.ToDouble(values[1]));
                ThermalEfficiency.Add(Convert.ToDouble(values[2]));
                Cost.Add(Convert.ToDouble(values[3]));
                CO2.Add(Convert.ToDouble(values[4]));
            }
            comboBox1.Items.AddRange(Technology.ToArray());
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
