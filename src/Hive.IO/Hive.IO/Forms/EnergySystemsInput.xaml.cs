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
                vm.ModuleType = vm.ModuleType;
                PropertiesView.ContentTemplate =
                    new PropertiesDataTemplateSelector().SelectTemplate(TechGrid.CurrentItem, TechGrid);
            }
        }

        private void TechGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropertiesView == null)
            {
                // still loading...
                return;
            }
            if (sender is DataGrid techGrid &&
                techGrid.DataContext is EnergySystemsInputViewModel esivm && techGrid.SelectedItem is ConversionTechPropertiesViewModel ctpvm)
            {
                InjectListOfAvailableSurfaces(ctpvm, esivm);
            }
        }

        private void InjectListOfAvailableSurfaces(ConversionTechPropertiesViewModel ctpvm, EnergySystemsInputViewModel esivm)
        {
            // inject list of surfaces available for this tech
            ctpvm.AvailableSurfaces = esivm.SurfacesForConversion(ctpvm);
            PropertiesView.ContentTemplate =
                new PropertiesDataTemplateSelector().SelectTemplate(TechGrid.CurrentItem, TechGrid);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (TechGrid.DataContext is EnergySystemsInputViewModel esivm && TechGrid.SelectedItem is ConversionTechPropertiesViewModel ctpvm)
            {
                InjectListOfAvailableSurfaces(ctpvm, esivm);
            }
        }
    }
}