using System;
using System.Collections.Generic;
using System.Linq;
using Hive.IO.EnergySystems;
using Hive.IO.Resources;
using Rhino.Geometry;

namespace Hive.IO.Forms
{
    public class ConversionTechPropertiesViewModel : ViewModelBase
    {
        private static Dictionary<string, ConversionTechDefaults> _defaults;
        private IEnumerable<SurfaceViewModel> _availableSurfaces;

        private string _endUse;

        private string _name;

        private string _source;

        private static Dictionary<string, ConversionTechDefaults> Defaults =>
            JsonResource.ReadRecords(ConversionTechDefaults.ResourceName, ref _defaults);

        public IEnumerable<string> ValidNames =>
            IsParametricDefined ? new List<string> {Name}.AsEnumerable() : Defaults.Keys;

        public IEnumerable<string> AllNames => Defaults.Keys;

        public string Name
        {
            get => _name ?? "Photovoltaic (PV)";
            set
            {
                if (AllNames.Contains(value))
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

        public string HeatToPowerRatio
        {
            get => $"{_heatToPowerRatio:0.00}";
            set => Set(ref _heatToPowerRatio, ParseDouble(value, _heatToPowerRatio));
        }


        public bool IsParametricDefined => ConversionTech != null;
        public ConversionTech ConversionTech { get; private set; }

        public bool IsEditable => !IsParametricDefined;

        public IEnumerable<SurfaceViewModel> AvailableSurfaces
        {
            get => _availableSurfaces;
            set => Set(ref _availableSurfaces, value);
        }

        public IEnumerable<SurfaceViewModel> SelectedSurfaces
        {
            get => _availableSurfaces?.Where(sm => sm.Connection == this) ?? new List<SurfaceViewModel>();
            set
            {
                SelectSurfaces(value);
                RaisePropertyChangedEvent();
            }
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
            var defaults = Defaults[Name];
            Source = defaults.Source;
            EndUse = defaults.EndUse;
        }

        private void SelectSurfaces(IEnumerable<SurfaceViewModel> surfaces)
        {
            foreach (var surface in _availableSurfaces) surface.Connection = null;
            foreach (var surface in surfaces) surface.Connection = this;
        }

        #region properties

        private double _efficiency;
        private double _capacity;
        private double _lifetime;
        private double _capitalCost;
        private double _operationalCost;
        private double _embodiedEmissions;
        private double _heatToPowerRatio;

        #endregion

        #region SetProperties

        /// <summary>
        ///     If this viewmodel was generated in the form itself, then this is null, else,
        ///     it's set to the ConversionTech object created in GhEnergySystems.SolveInstance.
        /// </summary>
        public void SetProperties(GasBoiler gasBoiler)
        {
            ConversionTech = gasBoiler;

            _efficiency = gasBoiler.Efficiency;
            _capacity = gasBoiler.Capacity;
            _capitalCost = gasBoiler.SpecificInvestmentCost;
            _embodiedEmissions = gasBoiler.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(HeatCoolingExchanger exchanger)
        {
            ConversionTech = exchanger;

            _efficiency = exchanger.DistributionLosses;
            _capacity = exchanger.Capacity;
            _capitalCost = exchanger.SpecificInvestmentCost;
            _embodiedEmissions = exchanger.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(Chiller chiller)
        {
            ConversionTech = chiller;

            _efficiency = chiller.EtaRef;
            _capacity = chiller.Capacity;
            _capitalCost = chiller.SpecificInvestmentCost;
            _embodiedEmissions = chiller.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(CombinedHeatPower chp)
        {
            ConversionTech = chp;

            _efficiency = chp.ElectricEfficiency;
            _capacity = chp.Capacity;
            _capitalCost = chp.SpecificInvestmentCost;
            _embodiedEmissions = chp.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
            _heatToPowerRatio = chp.HeatToPowerRatio;
        }

        public void SetProperties(AirSourceHeatPump ashp)
        {
            ConversionTech = ashp;

            _efficiency = ashp.EtaRef;
            _capacity = ashp.Capacity;
            _capitalCost = ashp.SpecificInvestmentCost;
            _embodiedEmissions = ashp.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(Photovoltaic photovoltaic)
        {
            ConversionTech = photovoltaic;

            _efficiency = photovoltaic.RefEfficiencyElectric;
            _capacity = photovoltaic.Capacity;
            _capitalCost = photovoltaic.SpecificInvestmentCost;
            _embodiedEmissions = photovoltaic.SpecificEmbodiedGhg;
            _lifetime = 9999999.0;
            _operationalCost = 999999.0;
        }

        public void SetProperties(SolarThermal solarThermal)
        {
            ConversionTech = solarThermal;

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

        public Mesh Mesh { get; set; }

        public ConversionTechPropertiesViewModel Connection { get; set; }
    }

    public class ConversionTechDefaults
    {
        public static string ResourceName = "Hive.IO.EnergySystems.conversion_technology_defaults.json";
        public string EndUse;
        public string Source;
    }
}