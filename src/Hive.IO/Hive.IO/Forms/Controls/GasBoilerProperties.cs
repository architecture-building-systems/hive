using System.ComponentModel;

namespace Hive.IO.Forms.Controls
{
    public partial class GasBoilerProperties : ConversionTechPropertiesBase
    {
        public GasBoilerProperties()
        {
            InitializeComponent();
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
        }
    }
}