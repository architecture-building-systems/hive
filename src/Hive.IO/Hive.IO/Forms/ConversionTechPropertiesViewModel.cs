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
                },
                {
                    "CHP", self =>
                    {
                        self.Source = "Gas";
                        self.EndUse = "Electricity demand, Heating demand, DHW";
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

        #region GasBoiler properties
        private double _efficiency;
        private double _capacity;
        private double _lifetime;
        private double _capitalCost;
        private double _operationalCost;
        private double _embodiedEmissions;
        #endregion

        public string Efficiency
        {
            get => $"{_efficiency:0.00}";
            set => Set(ref _efficiency, ParseDouble(value, _efficiency));
        }

        public string Capacity
        {
            get => $"{_capacity:0.00}";
            set => Set(ref _capacity, ParseDouble(value, _capacity));
        }

        public string Lifetime
        {
            get => $"{_lifetime:0}";
            set => Set(ref _lifetime, ParseDouble(value, _lifetime));
        }

        public string CapitalCost
        {
            get => $"{_capitalCost:0.00}";
            set => Set(ref _capitalCost, ParseDouble(value, _capitalCost));
        }

        public string OperationalCost
        {
            get => $"{_operationalCost:0.00}";
            set => Set(ref _operationalCost, ParseDouble(value, _operationalCost));
        }

        public string EmbodiedEmissions
        {
            get => $"{_embodiedEmissions:0.00}";
            set => Set(ref _embodiedEmissions, ParseDouble(value, _embodiedEmissions));
        }

        /// <summary>
        /// Parses the string to a double or returns the oldValue on error.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        private double ParseDouble(string value, double oldValue)
        {
            double result;
            if (double.TryParse(value, out result))
            {
                return result;
            }

            return oldValue;
        }

        /// <summary>
        ///     Assign default values based on the Name - gets called when setting the name.
        /// </summary>
        public void AssignDefaults()
        {
            Defaults[Name](this);
        }
    }
}