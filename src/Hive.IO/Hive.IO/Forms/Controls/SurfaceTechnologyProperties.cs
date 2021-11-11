using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Hive.IO.Forms.Controls
{
    public partial class SurfaceTechnologyProperties : ConversionTechPropertiesBase
    {
        private bool _initializingControls;

        public SurfaceTechnologyProperties()
        {
            InitializeComponent();
        }

        public override ConversionTechPropertiesViewModel Conversion
        {
            get => base.Conversion;
            set
            {
                base.Conversion = value;
                lblDescription.Text = Conversion.ModuleType.Description;
                technologyImage.Image = Conversion.TechnologyImage;

                UpdateAvailableSurfaces();
                UpdateModuleTypesList();
                UpdateCalculatedFields();
            }
        }

        /// <summary>
        ///     See [here](https://stackoverflow.com/a/3165330/2260) for why I'm not
        ///     hooking the event up directly with the handler defined in the base class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private new void Validating(object sender, CancelEventArgs e)
        {
            TextBox_Validating(sender, e);

            UpdateCalculatedFields();
        }

        private void UpdateAvailableSurfaces()
        {
            try
            {
                _initializingControls = true;

                lstAvailableSurfaces.Items.Clear();
                lstAvailableSurfaces.Items.AddRange(Conversion.AvailableSurfaces.ToArray<object>());

                for (var i = 0; i < lstAvailableSurfaces.Items.Count; i++)
                {
                    lstAvailableSurfaces.SetSelected(i,
                        Conversion.SelectedSurfaces.Contains(lstAvailableSurfaces.Items[i]));
                }
            }
            finally
            {
                _initializingControls = false;
            }
        }

        private void UpdateModuleTypesList()
        {
            try
            {
                _initializingControls = true;
                cboModuleType.Items.Clear();
                cboModuleType.Items.AddRange(
                    Conversion.ModuleTypes.Select(mt => mt.Name).ToArray<object>());
                cboModuleType.SelectedItem = Conversion.ModuleType.Name;
            }
            finally
            {
                _initializingControls = false;
            }
        }

        private void cboModuleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializingControls) return;

            var moduleName = cboModuleType.SelectedItem.ToString();
            var moduleType = Conversion.ModuleTypes.First(mt => mt.Name == moduleName);
            Conversion.ModuleType = moduleType;

            lblDescription.Text = Conversion.ModuleType.Description;
            technologyImage.Image = Conversion.TechnologyImage;

            foreach (var textBox in GetAll(this, typeof(TextBox)).Cast<TextBox>()) UpdateTextBoxText(textBox);
        }

        private void lstAvailableSurfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializingControls)
            {
                return;
            }

            Conversion.SelectedSurfaces = new List<SurfaceViewModel>(lstAvailableSurfaces.SelectedItems.Cast<SurfaceViewModel>());
            UpdateCalculatedFields();
        }

        private void lstAvailableSurfaces_SelectAll(object sender, KeyEventArgs e)
        {
            if (_initializingControls)
            {
                return;
            }

            if (e.Control && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < lstAvailableSurfaces.Items.Count; i++)
                {
                    lstAvailableSurfaces.SetSelected(i, true);
                }

                Conversion.SelectedSurfaces = new List<SurfaceViewModel>(lstAvailableSurfaces.SelectedItems.Cast<SurfaceViewModel>());
                UpdateCalculatedFields();
            }
        }

        private void UpdateCalculatedFields()
        {
            txtCapacity.Text = $"{Conversion.SurfaceTechCapacity:0.00}";
            txtEmbodiedEmissions.Text = $"{Conversion.EmbodiedEmissions:0.0}";
            txtCapitalCost.Text = $"{Conversion.CapitalCost:0.00}";
            txtArea.Text = $"{Conversion.Area:0.00}";
        }
    }
}