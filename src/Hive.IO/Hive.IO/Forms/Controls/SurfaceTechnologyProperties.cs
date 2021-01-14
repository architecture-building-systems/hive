using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Hive.IO.Forms.Controls
{
    public partial class SurfaceTechnologyProperties : ConversionTechPropertiesBase
    {
        public SurfaceTechnologyProperties()
        {
            InitializeComponent();
        }

        private bool _updatingModuleType;

        public override ConversionTechPropertiesViewModel Conversion { 
            get => base.Conversion;
            set
            {
                base.Conversion = value;
                lblDescription.Text = Conversion.ModuleType.Description;
                UpdateModuleTypesList();
            }
        }

        private void UpdateModuleTypesList()
        {
            try
            {
                _updatingModuleType = true;
                cboModuleType.Items.Clear();
                cboModuleType.Items.AddRange(Conversion.ModuleTypes.Select(mt => mt.Name).ToArray<object>());
                cboModuleType.SelectedItem = Conversion.ModuleType.Name;
            }
            finally
            {
                _updatingModuleType = false;
            }
        }

        /// <summary>
        ///     See [here](https://stackoverflow.com/a/3165330/2260) for why I'm not
        ///     hooking the event up directly with the handler defined in the base class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validating(object sender, CancelEventArgs e)
        {
            TextBox_Validating(sender, e);
        }

        private void cboModuleType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (_updatingModuleType) return;

            var moduleName = cboModuleType.SelectedItem.ToString();
            var moduleType = Conversion.ModuleTypes.First(mt => mt.Name == moduleName);
            Conversion.ModuleType = moduleType;

            lblDescription.Text = Conversion.ModuleType.Description;
            
            foreach (var textBox in GetAll(this, typeof(TextBox)).Cast<TextBox>()) UpdateTextBox(textBox);
        }
    }
}