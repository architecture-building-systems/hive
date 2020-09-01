using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hive.IO.Forms
{
    public class ConversionTechPropertiesViewModel : ViewModelBase
    {
        /// <summary>
        ///     A list of default values for the conversion technology properties
        /// </summary>
        private static readonly Dictionary<string, Action<ConversionTechPropertiesViewModel>> Defaults =
            new Dictionary<string, Action<ConversionTechPropertiesViewModel>>
            {
                {
                    "Photovoltaic (PV)", self =>
                    {
                        self.Source = "Solar";
                        self.EndUse = "Electricity demand";
                    }
                },
                {
                    "Boiler (Gas)", self =>
                    {
                        self.Source = "Gas";
                        self.EndUse = "Heating demand, DHW";
                    }
                }
            };

        public static IEnumerable<string> ValidNames => Defaults.Keys;

        private string _name;
        public string Name
        {
            get => _name ?? "Photovoltaic (PV)";
            set
            {
                if (ValidNames.Contains(value))
                {
                    Set(ref _name, value);
                    AssignDefaults();
                }
                else
                {
                    throw new ArgumentException($"Invalid ConversionTechPropertiesViewModel.Name: {value}");
                }
            }
        }

        private string _source;
        public string Source { get => _source; set => Set(ref _source, value); }
        private string _endUse;
        public string EndUse { get => _endUse; set => Set(ref _endUse, value); }

        /// <summary>
        ///     Assign default values based on the Name - gets called when setting the name.
        /// </summary>
        public void AssignDefaults()
        {
            Defaults[Name](this);
        }
    }
}