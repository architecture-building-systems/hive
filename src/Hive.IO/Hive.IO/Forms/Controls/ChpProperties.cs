using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hive.IO.Forms.Controls
{
    public partial class ChpProperties : ConversionTechPropertiesBase
    {
        public ChpProperties()
        {
            InitializeComponent();
        }

        /// <summary>
        /// See [here](https://stackoverflow.com/a/3165330/2260) for why I'm not
        /// hooking the event up directly with the handler defined in the base class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox_Validating(sender, e);
        }
    }
}
