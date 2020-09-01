using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Hive.IO.Forms
{
    /// <summary>
    ///     Interaction logic for EnergySystemsInput.xaml
    /// </summary>
    public partial class EnergySystemsInput
    {
        public EnergySystemsInput()
        {
            InitializeComponent();
        }

        private EnergySystemsInputViewModel ViewModel => (EnergySystemsInputViewModel) DataContext;

        public IEnumerable<ConversionTechPropertiesViewModel> Conversions => ViewModel.ConversionTechnologies;

        private void ConversionNames_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox conversionNames &&
                conversionNames.DataContext is ConversionTechPropertiesViewModel vm)
            {
                vm.Name = (string) conversionNames.SelectedValue;
                PropertiesView.ContentTemplate =
                    new PropertiesDataTemplateSelector().SelectTemplate(TechGrid.CurrentItem, TechGrid);
            }
        }

        private void TechGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                PropertiesView.ContentTemplate =
                    new PropertiesDataTemplateSelector().SelectTemplate(dataGrid.CurrentItem, dataGrid);
            }
        }

        private void TechGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                PropertiesView.ContentTemplate =
                    new PropertiesDataTemplateSelector().SelectTemplate(dataGrid.CurrentItem, dataGrid);
            }
        }
    }
}