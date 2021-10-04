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
                UpdateArea();
            }
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
            UpdateArea();
        }

        private void UpdateArea()
        {
            txtArea.Text = $"{Conversion.Area:0.00}";
        }
    }
}