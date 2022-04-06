using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hive.IO.Forms
{
    public partial class VisualizerPlotProperties : Form
    {
        private Dictionary<string, string> _plotParameters;

        private int locationX;
        private int locationY;
        public VisualizerPlotProperties()
        {
            this.locationX = Cursor.Position.X;
            this.locationY = Cursor.Position.Y;
            Load += new EventHandler(VisualizerPlotProperties_Load);
            InitializeComponent();
        }

        private void VisualizerPlotProperties_Load(object sender, System.EventArgs e)
        {
            this.SetDesktopLocation(locationX, locationY);
        }

        /// <summary>
        /// The plot parameters from the GhVisualizer. This is set before the form
        /// is shown and read after it closes.
        /// </summary>
        public Dictionary<string, string> PlotParameters
        {
            get => _plotParameters;
            set
            {
                _plotParameters = value;

                // add the data to the controls
                txtEnergyDemandMinimum.Text = ReadParameter("EnergyDemandMonthly-Axis-Minimum");
                txtEnergyDemandMaximum.Text = ReadParameter("EnergyDemandMonthly-Axis-Maximum");
                txtSolarGainsMinimum.Text = ReadParameter("SolarGains-Axis-Minimum");
                txtSolarGainsMaximum.Text = ReadParameter("SolarGains-Axis-Maximum");
            }
        }

        private string ReadParameter(string name) => _plotParameters.ContainsKey(name) ? _plotParameters[name] : "";

        private void WriteDoubleParameter(string name, string value)
        {
            double parsedValue;
            if (double.TryParse(value, out parsedValue))
            {
                _plotParameters[name] = $"{parsedValue:0.00}";
            }
            else
            {
                _plotParameters[name] = "";
            }
        }

        /// <summary>
        /// Save the new values back to the dictionary. Take care to store correct values (doubles or "").
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteDoubleParameter("EnergyDemandMonthly-Axis-Minimum", txtEnergyDemandMinimum.Text);
            WriteDoubleParameter("EnergyDemandMonthly-Axis-Maximum", txtEnergyDemandMaximum.Text);
            WriteDoubleParameter("SolarGains-Axis-Minimum", txtSolarGainsMinimum.Text);
            WriteDoubleParameter("SolarGains-Axis-Maximum", txtSolarGainsMaximum.Text);
        }

        private void btnResetDemandAxis_Click(object sender, EventArgs e)
        {
            txtEnergyDemandMinimum.Text = null;
            txtEnergyDemandMaximum.Text = null;
        }

        private void btnResetSolarGainsAxis_Click(object sender, EventArgs e)
        {
            txtSolarGainsMinimum.Text = null;
            txtSolarGainsMaximum.Text = null;
        }
    }
}
