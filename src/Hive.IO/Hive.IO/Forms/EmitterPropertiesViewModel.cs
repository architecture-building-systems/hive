﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Hive.IO.EnergySystems;
using Hive.IO.Util;

namespace Hive.IO.Forms
{
    public class EmitterPropertiesViewModel : ViewModelBase
    {
        #region backing variables

        private string _name;

        private double _supplyTemperature;
        private double _returnTemperature;
        private double _specificCapitalCost;
        private double _specificEmbodiedEmissions;
        private double _lifetime;
        private double _capacity;
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
            get => _name ?? "Radiator";
            set
            {
                if (AllNames.Contains(value))
                {
                    _name = value;
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
            _capacity = defaults.Capacity;
            _specificCapitalCost = defaults.SpecificCapitalCost;
            _specificEmbodiedEmissions = defaults.SpecificEmbodiedEmissions;
            _lifetime = defaults.Lifetime;
            _isAir = defaults.IsAir;
            _isRadiation = defaults.IsRadiation;
            _isHeating = defaults.IsHeating;
            _isCooling = defaults.IsCooling;
        }

        public void SetProperties(Emitter emitter)
        {
            Emitter = emitter;

            _supplyTemperature = emitter.InletCarrier.Temperature.Average();
            _returnTemperature = emitter.ReturnCarrier.Temperature.Average();
            _capacity = 0.00;
            _specificCapitalCost = emitter.SpecificInvestmentCost;
            _specificEmbodiedEmissions = emitter.SpecificEmbodiedGhg;
            _lifetime = emitter.Lifetime;
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
            set => _supplyTemperature = ParseDouble(value, _supplyTemperature);
        }

        public string ReturnTemperature
        {
            get => $"{_returnTemperature:0.00}";
            set => _returnTemperature = ParseDouble(value, _returnTemperature);
        }

        public string Capacity
        {
            get => $"{_capacity:0.00}";
            set => _capacity= ParseDouble(value, _capacity);
        }

        public string SpecificCapitalCost
        {
            get => $"{_specificCapitalCost:0.00}";
            set => _specificCapitalCost = ParseDouble(value, _specificCapitalCost);
        }


        public string SpecificEmbodiedEmissions
        {
            get => $"{_specificEmbodiedEmissions:0.00}";
            set => _specificEmbodiedEmissions = ParseDouble(value, _specificEmbodiedEmissions);
        }

        public string Lifetime
        {
            get => $"{_lifetime:0.00}";
            set => _lifetime = ParseDouble(value, _lifetime);
        }

        public double EmbodiedEmissions => _capacity * _specificEmbodiedEmissions;
        public double CapitalCost => _capacity * _specificCapitalCost;

        public bool IsAir
        {
            get => _isAir;
            set => _isAir= value;
        }

        public bool IsRadiation
        {
            get => _isRadiation;
            set => _isRadiation= value;
        }

        public bool IsCooling
        {
            get => _isCooling;
            set => _isCooling= value;
        }

        public bool IsHeating
        {
            get => _isHeating;
            set => _isHeating= value;
        }

        #region FontWeights and Colors
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
            return Emitter == null ? AreEqual(a, b) ? _normalBrush : _modifiedBrush : _normalBrush;
        }

        private FontWeight CompareFontWeight(double a, double b)
        {
            return Emitter == null
                ? AreEqual(a, b) ? _normalFontWeight : _modifiedFontWeight
                : _normalFontWeight;
        }

        public Brush SupplyTemperatureBrush => CompareBrush(_supplyTemperature, Defaults[Name].SupplyTemperature);
        public Brush ReturnTemperatureBrush => CompareBrush(_returnTemperature, Defaults[Name].ReturnTemperature);
        public Brush CapacityBrush => CompareBrush(_capacity, Defaults[Name].Capacity);

        public Brush SpecificCapitalCostBrush => CompareBrush(_specificCapitalCost, Defaults[Name].SpecificCapitalCost);
        public Brush SpecificEmbodiedEmissionsBrush => CompareBrush(_specificEmbodiedEmissions, Defaults[Name].SpecificEmbodiedEmissions);
        public Brush LifetimeBrush => CompareBrush(_lifetime, Defaults[Name].Lifetime);


        public FontWeight SupplyTemperatureFontWeight => CompareFontWeight(_supplyTemperature, Defaults[Name].SupplyTemperature);

        public FontWeight ReturnTemperatureFontWeight =>
            CompareFontWeight(_returnTemperature, Defaults[Name].ReturnTemperature);
        public FontWeight SpecificCapitalCostFontWeight =>
            CompareFontWeight(_specificCapitalCost, Defaults[Name].SpecificCapitalCost);
        
        public FontWeight SpecificEmbodiedEmissionsFontWeight =>
            CompareFontWeight(_specificEmbodiedEmissions, Defaults[Name].SpecificEmbodiedEmissions);

        public FontWeight LifetimeFontWeight => CompareFontWeight(_lifetime, Defaults[Name].Lifetime);

        public FontWeight CapacityFontWeight => CompareFontWeight(_capacity, Defaults[Name].Capacity);
        #endregion


        private static Dictionary<string, EmitterDefaults> Defaults =>
            JsonResource.ReadRecords(EmitterDefaults.ResourceName, ref _defaults);

        public IEnumerable<string> ValidNames =>
            IsParametricDefined ? new List<string> { Name }.AsEnumerable() : Defaults.Keys;

        public bool IsParametricDefined => Emitter != null;
        public bool IsEditable => !IsParametricDefined;

        public Emitter Emitter { get; private set; }
    }

    public struct EmitterDefaults
    {
        public static string ResourceName = "Hive.IO.EnergySystems.emitter_defaults.json";

        public double SupplyTemperature;
        public double ReturnTemperature;
        public double Capacity;
        public double SpecificCapitalCost;
        public double SpecificEmbodiedEmissions;
        public double Lifetime;
        public bool IsAir;
        public bool IsRadiation;
        public bool IsCooling;
        public bool IsHeating;
    }
}