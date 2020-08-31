using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hive.IO.EnergySystems;

namespace Hive.IO.Forms
{
    /// <summary>
    /// A ViewModel to bind the EnergySystemsInput form to. This includes any defaults used and relationships to user controls (ConversionProperties).
    /// </summary>
    public class EnergySystemsInputViewModel: ViewModelBase
    {
        private IConversionTechProperties _currentConversionTechProperties;

        public IConversionTechProperties CurrentConversionTechProperties
        {
            get => _currentConversionTechProperties;
            private set => Set(ref _currentConversionTechProperties, value);
        }

        private Dictionary<string, Func<IConversionTechProperties>> _conversions =
            new Dictionary<string, Func<IConversionTechProperties>>
            {
                {"Photovoltaic (PV)", () => new PhotovoltaicPropertiesViewModel()},
                {"Boiler (Gas)", () => new GasBoilerPropertiesViewModel()}
            };

        public IEnumerable<string> Conversions => _conversions.Keys;

        public ObservableCollection<IConversionTechProperties> ConversionTechnologies { get; private set; }

        public EnergySystemsInputViewModel()
        {
            ConversionTechnologies = new ObservableCollection<IConversionTechProperties>();
        }
    }
}
