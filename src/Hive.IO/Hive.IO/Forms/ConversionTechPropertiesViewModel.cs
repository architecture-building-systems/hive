using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Hive.IO.EnergySystems;
using Rhino.Geometry;

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
                    "Solar Thermal (ST)", self =>
                    {
                        self.Source = "Solar";
                        self.EndUse = "Heating demand";
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
                },
                {
                    "Chiller (Electricity)", self =>
                    {
                        self.Source = "Electricity Grid";
                        self.EndUse = "Cooling demand";
                    }
                },
                {
                    "ASHP (Electricity)", self =>
                    {
                        self.Source = "Air";
                        self.EndUse = "Electricity demand";
                    }
                },
                {
                    "Heat Exchanger", self =>
                    {
                        self.Source = "Air";
                        self.EndUse = "Electricity demand";
                    }
                },
                {
                    "Cooling Exchanger", self =>
                    {
                        self.Source = "Air";
                        self.EndUse = "Electricity demand";
                    }
                }
            };

        private string _endUse;

        private string _name;

        private string _source;

        public static IEnumerable<string> ValidNames => Defaults.Keys;

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

        public string Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public string EndUse
        {
            get => _endUse;
            set => Set(ref _endUse, value);
        }

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
        ///     Parses the string to a double or returns the oldValue on error.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        private double ParseDouble(string value, double oldValue)
        {
            double result;
            if (double.TryParse(value, out result)) return result;

            return oldValue;
        }

        /// <summary>
        ///     Assign default values based on the Name - gets called when setting the name.
        /// </summary>
        public void AssignDefaults()
        {
            Defaults[Name](this);
        }

        private IEnumerable<SurfaceViewModel> _availableSurfaces;
        public IEnumerable<SurfaceViewModel> AvailableSurfaces { get => _availableSurfaces; set => Set(ref _availableSurfaces, value); }

        private void SelectSurfaces(IEnumerable<SurfaceViewModel> surfaces)
        {
            foreach (var surface in _availableSurfaces)
            {
                surface.Connection = null;
            }
            foreach (var surface in surfaces)
            {
                surface.Connection = this;
            }
        }
        public IEnumerable<SurfaceViewModel> SelectedSurfaces
        {
            get => _availableSurfaces?.Where(sm => sm.Connection == this) ?? new List<SurfaceViewModel>();
            set { SelectSurfaces(value); RaisePropertyChangedEvent(); }
        }

        #region properties

        private double _efficiency;
        private double _capacity;
        private double _lifetime;
        private double _capitalCost;
        private double _operationalCost;
        private double _embodiedEmissions;

        #endregion

        #region SetProperties

        public void SetProperties(GasBoiler gasBoiler)
        {
            _efficiency = gasBoiler.Efficiency;
            _capacity = gasBoiler.Capacity;
            _capitalCost = gasBoiler.SpecificInvestmentCost;
            _embodiedEmissions = gasBoiler.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(HeatCoolingExchanger exchanger)
        {
            _efficiency = exchanger.DistributionLosses;
            _capacity = exchanger.Capacity;
            _capitalCost = exchanger.SpecificInvestmentCost;
            _embodiedEmissions = exchanger.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(Chiller chiller)
        {
            _efficiency = chiller.EtaRef;
            _capacity = chiller.Capacity;
            _capitalCost = chiller.SpecificInvestmentCost;
            _embodiedEmissions = chiller.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(CombinedHeatPower chp)
        {
            _efficiency = chp.ElectricEfficiency;
            _capacity = chp.Capacity;
            _capitalCost = chp.SpecificInvestmentCost;
            _embodiedEmissions = chp.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(AirSourceHeatPump ashp)
        {
            _efficiency = ashp.EtaRef;
            _capacity = ashp.Capacity;
            _capitalCost = ashp.SpecificInvestmentCost;
            _embodiedEmissions = ashp.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(Photovoltaic photovoltaic)
        {
            _efficiency = photovoltaic.RefEfficiencyElectric;
            _capacity = photovoltaic.Capacity;
            _capitalCost = photovoltaic.SpecificInvestmentCost;
            _embodiedEmissions = photovoltaic.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(SolarThermal solarThermal)
        {
            _efficiency = solarThermal.RefEfficiencyHeating;
            _capacity = solarThermal.Capacity;
            _capitalCost = solarThermal.SpecificInvestmentCost;
            _embodiedEmissions = solarThermal.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        #endregion
    }

    public class SurfaceViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public double Area { get; set; }

        public ConversionTechPropertiesViewModel Connection { get; set; }
    }
}