using System.Windows;
using System.Windows.Controls;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Interaction logic for EnergySystemsInput.xaml
    /// </summary>
    public partial class EnergySystemsInput
    {
        public EnergySystemsInput()
        {
            InitializeComponent();
        }

        private void ConversionNames_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox conversionNames && conversionNames.DataContext is ConversionTechPropertiesViewModel vm)
            {
                vm.Name = (string)conversionNames.SelectedValue;
            }
        }
    }
}
