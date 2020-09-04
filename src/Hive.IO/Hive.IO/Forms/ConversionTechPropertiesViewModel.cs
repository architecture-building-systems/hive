using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Hive.IO.EnergySystems;
using Hive.IO.Resources;
using Rhino.Geometry;

namespace Hive.IO.Forms
{
    public class ConversionTechPropertiesViewModel : ViewModelBase
    {
        private static Dictionary<string, ConversionTechDefaults> _defaults;

        private static Dictionary<string, List<ModuleTypeRecord>> _moduleTypesCatalog;
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
            set => SetWithColors(ref _efficiency, ParseDouble(value, _efficiency));
        }

        public string Capacity
        {
            get => $"{_capacity:0.00}";
            set => SetWithColors(ref _capacity, ParseDouble(value, _capacity));
        }

        public string Lifetime
        {
            get => $"{_lifetime:0}";
            set => SetWithColors(ref _lifetime, ParseDouble(value, _lifetime));
        }

        public string CapitalCost
        {
            get => $"{_capitalCost:0.00}";
            set => SetWithColors(ref _capitalCost, ParseDouble(value, _capitalCost));
        }

        public string OperationalCost
        {
            get => $"{_operationalCost:0.00}";
            set => SetWithColors(ref _operationalCost, ParseDouble(value, _operationalCost));
        }

        public string EmbodiedEmissions
        {
            get => $"{_embodiedEmissions:0.00}";
            set => SetWithColors(ref _embodiedEmissions, ParseDouble(value, _embodiedEmissions));
        }

        public string HeatToPowerRatio
        {
            get => $"{_heatToPowerRatio:0.00}";
            set => SetWithColors(ref _heatToPowerRatio, ParseDouble(value, _heatToPowerRatio));
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

        public IEnumerable<ModuleTypeRecord> ModuleTypes
        {
            get
            {
                if (!IsSurfaceTech)
                {
                    return new List<ModuleTypeRecord>().AsEnumerable();
                }

                if (!IsParametricDefined)
                {
                    return ModuleTypesCatalog[Name].AsEnumerable();
                }

                // parametric surface tech - it was set in SetProperties...
                return new List<ModuleTypeRecord>{_moduleType}.AsEnumerable();
            }
        }

        public ModuleTypeRecord ModuleType
        {
            get => _moduleType;
            set
            {
                if (IsSurfaceTech && ModuleTypesCatalog[Name].Contains(value))
                {
                    _moduleType = value;
                    switch (Name)
                    {
                        case "Photovoltaic (PV)":
                            _efficiency = _moduleType.ElectricEfficiency;
                            break;
                        case "Solar Thermal (ST)":
                            _efficiency = _moduleType.ThermalEfficiency;
                            break;
                    }

                    var area = SelectedSurfaces.Sum(sm => sm.Area);
                    _capitalCost = _moduleType.SpecificCapitalCost * area;
                    _embodiedEmissions = _moduleType.SpecificEmbodiedEmissions * area;
                    
                    // make sure everyone knows about this!
                    RaisePropertyChangedEvent();
                    RaisePropertyChangedEvent("Efficiency");
                    RaisePropertyChangedEvent("EmbodiedEmissions");
                    RaisePropertyChangedEvent("CapitalCost");
                }
            }
        }

        private bool IsSurfaceTech => Name == "Photovoltaic (PV)" || Name == "Solar Thermal (ST)";

        private static Dictionary<string, List<ModuleTypeRecord>> ModuleTypesCatalog =>
            JsonResource.ReadRecords(ModuleTypeRecord.ResourceName, ref _moduleTypesCatalog);

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

            _efficiency = defaults.Efficiency;
            _capacity = defaults.Capacity;
            _capitalCost = defaults.CapitalCost;
            _embodiedEmissions = defaults.EmbodiedEmissions;
            _lifetime = defaults.Lifetime;
            _operationalCost = defaults.OperationalCost;

            if (IsSurfaceTech)  // NOTE: yep. this should be a subclass. maybe we'll fix it someday.
            {
                ModuleType = ModuleTypesCatalog.ContainsKey(Name) ? ModuleTypesCatalog[Name].First() : new ModuleTypeRecord { Name = "<custom>" };
            }
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
        private ModuleTypeRecord _moduleType;

        #endregion

        #region Highlighting differences

        private readonly Brush _normalBrush = new SolidColorBrush(Colors.Black);
        private readonly Brush _modifiedBrush = new SolidColorBrush(Colors.ForestGreen);
        private readonly FontWeight _normalFontWeight = FontWeights.Normal;
        private readonly FontWeight _modifiedFontWeight = FontWeights.Bold;

        private bool AreEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.001;
        }

        private Brush CompareBrush(double a, double b)
        {
            return ConversionTech == null ? AreEqual(a, b) ? _normalBrush : _modifiedBrush : _normalBrush;
        }

        private FontWeight CompareFontWeight(double a, double b)
        {
            return ConversionTech == null
                ? AreEqual(a, b) ? _normalFontWeight : _modifiedFontWeight
                : _normalFontWeight;
        }

        public Brush EfficiencyBrush => CompareBrush(_efficiency, Defaults[Name].Efficiency);
        public Brush CapacityBrush => CompareBrush(_capacity, Defaults[Name].Capacity);
        public Brush LifetimeBrush => CompareBrush(_lifetime, Defaults[Name].Lifetime);
        public Brush CapitalCostBrush => CompareBrush(_capitalCost, Defaults[Name].CapitalCost);
        public Brush OperationalCostBrush => CompareBrush(_operationalCost, Defaults[Name].OperationalCost);
        public Brush EmbodiedEmissionsBrush => CompareBrush(_embodiedEmissions, Defaults[Name].EmbodiedEmissions);
        public Brush HeatToPowerRatioBrush => CompareBrush(_heatToPowerRatio, Defaults[Name].HeatToPowerRatio);

        public FontWeight EfficiencyFontWeight => CompareFontWeight(_efficiency, Defaults[Name].Efficiency);
        public FontWeight CapacityFontWeight => CompareFontWeight(_capacity, Defaults[Name].Capacity);
        public FontWeight LifetimeFontWeight => CompareFontWeight(_lifetime, Defaults[Name].Lifetime);
        public FontWeight CapitalCostFontWeight => CompareFontWeight(_capitalCost, Defaults[Name].CapitalCost);

        public FontWeight OperationalCostFontWeight =>
            CompareFontWeight(_operationalCost, Defaults[Name].OperationalCost);

        public FontWeight EmbodiedEmissionsFontWeight =>
            CompareFontWeight(_embodiedEmissions, Defaults[Name].EmbodiedEmissions);

        public FontWeight HeatToPowerRatioFontWeight =>
            CompareFontWeight(_heatToPowerRatio, Defaults[Name].HeatToPowerRatio);

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
            _moduleType = new ModuleTypeRecord
            {
                Name = "<custom>",
                ElectricEfficiency = photovoltaic.RefEfficiencyElectric,
                SpecificCapitalCost = photovoltaic.SpecificInvestmentCost,
                SpecificEmbodiedEmissions = photovoltaic.SpecificEmbodiedGhg,
                ThermalEfficiency = 0.00
            };
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

            _moduleType = new ModuleTypeRecord
            {
                Name = "<custom>",
                ElectricEfficiency = 0.00,
                SpecificCapitalCost = solarThermal.SpecificInvestmentCost,
                SpecificEmbodiedEmissions = solarThermal.SpecificEmbodiedGhg,
                ThermalEfficiency = solarThermal.RefEfficiencyHeating
            };
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

    public struct ConversionTechDefaults
    {
        public static string ResourceName = "Hive.IO.EnergySystems.conversion_technology_defaults.json";
        public string EndUse;
        public string Source;
        public double Efficiency;
        public double Capacity;
        public double CapitalCost;
        public double Lifetime;
        public double OperationalCost;
        public double EmbodiedEmissions;
        public double HeatToPowerRatio;
    }

    public struct ModuleTypeRecord
    {
        public static string ResourceName = "Hive.IO.EnergySystems.surface_tech_module_types.json";
        public string Name { get; set; }
        public double ElectricEfficiency;
        public double ThermalEfficiency;
        public double SpecificCapitalCost;
        public double SpecificEmbodiedEmissions;
    }
}