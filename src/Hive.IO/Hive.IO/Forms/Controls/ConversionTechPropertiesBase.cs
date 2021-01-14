using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;

namespace Hive.IO.Forms.Controls
{
    /// <summary>
    ///     Base UserControl class with methods used for connecting to a ConversionTechPropertiesViewModel.
    /// </summary>
    public partial class ConversionTechPropertiesBase : UserControl
    {
        private ConversionTechPropertiesViewModel _conversion;
        private bool _rendering;

        public ConversionTechPropertiesBase()
        {
            InitializeComponent();
        }

        public virtual ConversionTechPropertiesViewModel Conversion
        {
            get => _conversion;
            set
            {
                _conversion = value;
                RenderState();
            }
        }

        private void RenderState()
        {
            try
            {
                _rendering = true;
                foreach (var textBox in GetAll(this, typeof(TextBox)).Cast<TextBox>()) UpdateTextBox(textBox);
            }
            finally
            {
                _rendering = false;
            }
        }

        /// <summary>
        ///     Find all controls of a given type using code from here:
        ///     https://stackoverflow.com/a/3426721/2260
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);
        }

        /// <summary>
        ///     Use Reflection to update the textbox values based on the current state,
        ///     including font weight and text color.
        ///     Note that the ViewModel (ConversionTechPropertiesViewModel) was originally written for WPF,
        ///     so we need to convert the FontWeight and SolidColorBrush to the Windows.Forms world.
        /// </summary>
        /// <param name="textBox"></param>
        protected void UpdateTextBox(TextBox textBox)
        {
            if (textBox.Tag == null)
            {
                return;
            }

            var property = textBox.Tag.ToString();
            textBox.Text = Conversion.GetType().GetProperty(property).GetValue(Conversion) as string;
            textBox.Enabled = Conversion.IsEditable;

            if (Conversion.IsEditable && Conversion.GetType().GetProperty(property + "FontWeight") != null)
            {
                var fontWeight =
                    (FontWeight) Conversion.GetType().GetProperty(property + "FontWeight").GetValue(Conversion);
                textBox.Font = new Font(textBox.Font,
                    fontWeight == FontWeights.Bold ? FontStyle.Bold : FontStyle.Regular);

                var solidBrush =
                    (SolidColorBrush) Conversion.GetType().GetProperty(property + "Brush").GetValue(Conversion);
                var foreColor = Color.FromArgb(
                    solidBrush.Color.A,
                    solidBrush.Color.R,
                    solidBrush.Color.G,
                    solidBrush.Color.B);
                textBox.ForeColor = foreColor;
            }
            else
            {
                // this value is never editable, that's why it doesn't have a "FontWeight"
                textBox.Enabled = false;
            }
        }


        /// <summary>
        ///     The text in a textbox was changed - each textbox hooked up with this handler needs to have
        ///     the "Tag" property set to the name of the corresponding ConversionTechPropertiesViewModel property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (_rendering) return;

            var textBox = (TextBox) sender;
            var stateProperty = textBox.Tag.ToString();

            Conversion.GetType().GetProperty(stateProperty).SetValue(Conversion, textBox.Text);

            RenderState();
        }
    }
}