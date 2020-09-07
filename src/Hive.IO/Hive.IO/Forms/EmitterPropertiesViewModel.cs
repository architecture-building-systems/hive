using System;
using System.Collections.Generic;
using System.Linq;
using Hive.IO.EnergySystems;
using Hive.IO.Resources;

namespace Hive.IO.Forms
{
    public class EmitterPropertiesViewModel : ViewModelBase
    {
        #region backing variables

        private string _name;

        private double _supplyTemperature;
        private double _returnTemperature;
        private double _lifetime;
        private double _capacity;
        private double _capitalCost;
        private double _operationalCost;
        private double _embodiedEmissions;
        private bool _isAir;
        private bool _isRadiation;
        private bool _isCooling;
        private bool _isHeating;

        private static Dictionary<string, EmitterDefaults> _defaults;

        #endregion

        /// <summary>
        /// Setting the Name is special, as it pre-selects default values from the database...
        /// </summary>
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
        /// <summary>
        ///     Assign default values based on the Name - gets called when setting the name.
        /// </summary>
        public void AssignDefaults()
        {
            var defaults = Defaults[Name];

            _supplyTemperature = defaults.SupplyTemperature;
            _returnTemperature = defaults.ReturnTemperature;
            _lifetime = defaults.Lifetime;
            _capacity = defaults.Capacity;
            _capitalCost = defaults.CapitalCost;
            _operationalCost = defaults.OperationalCost;
            _embodiedEmissions = defaults.EmbodiedEmissions;
            _isAir = defaults.IsAir;
            _isRadiation = defaults.IsRadiation;
            _isHeating = defaults.IsHeating;
            _isCooling = defaults.IsCooling;

            RaisePropertyChangedEvent(null);
        }

        public void SetProperties(Emitter emitter)
        {
            Emitter = emitter;

            _supplyTemperature = emitter.InletCarrier.Temperature.Average();
            _returnTemperature = emitter.ReturnCarrier.Temperature.Average();

            _lifetime = 0.00;
            _capacity = 0.00;
            _capitalCost = emitter.SpecificInvestmentCost;

            _operationalCost = 0.00;
            _embodiedEmissions = emitter.SpecificEmbodiedGhg;

            _isAir = emitter is AirDiffuser;
            _isRadiation = emitter is Radiator;
            _isHeating = emitter.IsHeating;
            _isCooling = emitter.IsCooling;
        }

        public static IEnumerable<string> AllNames => Defaults.Keys;

        public string EndUse
        {
            get
            {
                if (_isHeating && _isCooling)
                {
                    return "Heating demand, Cooling demand";
                }

                if (_isHeating)
                {
                    return "Heating demand";
                }

                if (_isCooling)
                {
                    return "Cooling demand";
                }

                return "???";
            }
        }

        public string SupplyTemperature
        {
            get => $"{_supplyTemperature:0.00}";
            set => SetWithColors(ref _supplyTemperature, ParseDouble(value, _operationalCost));
        }

        public string ReturnTemperature
        {
            get => $"{_returnTemperature:0.00}";
            set => SetWithColors(ref _returnTemperature, ParseDouble(value, _operationalCost));
        }

        public string Lifetime
        {
            get => $"{_lifetime:0}";
            set => SetWithColors(ref _lifetime, ParseDouble(value, _operationalCost));
        }

        public string Capacity
        {
            get => $"{_capacity:0.00}";
            set => SetWithColors(ref _capacity, ParseDouble(value, _operationalCost));
        }

        public string CapitalCost
        {
            get => $"{_capitalCost:0.00}";
            set => SetWithColors(ref _capitalCost, ParseDouble(value, _operationalCost));
        }

        public string OperationalCost
        {
            get => $"{_operationalCost:0.00}";
            set => SetWithColors(ref _operationalCost, ParseDouble(value, _operationalCost));
        }

        public string EmbodiedEmissions
        {
            get => $"{_embodiedEmissions:0.00}";
            set => SetWithColors(ref _embodiedEmissions, ParseDouble(value, _operationalCost));
        }

        public bool IsAir
        {
            get => _isAir;
            set => SetWithColors(ref _isAir, value);
        }

        public bool IsRadiation
        {
            get => _isRadiation;
            set => SetWithColors(ref _isRadiation, value);
        }

        public bool IsCooling
        {
            get => _isCooling;
            set => SetWithColors(ref _isCooling, value);
        }

        public bool IsHeating
        {
            get => _isHeating;
            set => SetWithColors(ref _isHeating, value);
        }



        private static Dictionary<string, EmitterDefaults> Defaults =>
            JsonResource.ReadRecords(EmitterDefaults.ResourceName, ref _defaults);

        public IEnumerable<string> ValidNames =>
            IsParametricDefined ? new List<string> { Name }.AsEnumerable() : Defaults.Keys;

        public bool IsParametricDefined => Emitter != null;
        public Emitter Emitter { get; private set; }
    }

    public struct EmitterDefaults
    {
        public static string ResourceName = "Hive.IO.EnergySystems.emitter_defaults.json";

        public double SupplyTemperature;
        public double ReturnTemperature;
        public double Lifetime;
        public double Capacity;
        public double CapitalCost;
        public double OperationalCost;
        public double EmbodiedEmissions;
        public bool IsAir;
        public bool IsRadiation;
        public bool IsCooling;
        public bool IsHeating;
    }
}