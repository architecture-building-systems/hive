using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Hive.IO.EnergySystems;

namespace Hive.IO.Forms
{
    /// <summary>
    /// A ViewModel to bind the EnergySystemsInput form to. This includes any defaults used and relationships to user controls (ConversionProperties).
    /// </summary>
    public class EnergySystemsInputViewModel: ViewModelBase
    {
        public IEnumerable<string> ConversionNames => ConversionTechPropertiesViewModel.ValidNames;

        public ObservableCollection<ConversionTechPropertiesViewModel> ConversionTechnologies { get; }

        public ICollectionView ConversionTechnologiesView =>
            CollectionViewSource.GetDefaultView(ConversionTechnologies);

        public EnergySystemsInputViewModel()
        {
            ConversionTechnologies = new ObservableCollection<ConversionTechPropertiesViewModel>
            {
                new ConversionTechPropertiesViewModel()
                {
                    Name = "Photovoltaic (PV)",
                },
                new ConversionTechPropertiesViewModel()
                {
                    Name = "Boiler (Gas)",
                }
            };
        }
    }
}
