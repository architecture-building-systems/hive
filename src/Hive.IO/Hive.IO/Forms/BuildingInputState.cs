using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Hive.IO.Building;

namespace Hive.IO.Forms
{
    /// <summary>
    ///     Capture the state of the BuildingInput form... allow binding controls to manipulate the _siaRoom...
    /// </summary>
    public class BuildingInputState : INotifyPropertyChanged
    {
        private bool _editable;
        private Sia2024RecordEx _siaRoom;
        private Zone _zone;


        public BuildingInputState(Sia2024RecordEx room, Zone zone, bool editable)
        {
            _siaRoom = room.Clone();
            _editable = editable;
            _zone = zone.Clone();
        }

        public Sia2024RecordEx SiaRoom
        {
            get => _siaRoom;
            set
            {
                _siaRoom = value;
                RaiseAllPropertiesChangedEvent();
            }
        }

        public Zone Zone
        {
            get => _zone;
            set
            {
                _zone = value;
                RaiseAllPropertiesChangedEvent();
            }
        }

        public bool IsEditable
        {
            get => _editable;
            set
            {
                _editable = value;
                RaisePropertyChangedEvent(null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Notify the GUI that not only has the property changed, but all the brushes and fonts changed too.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void RaisePropertyChangedEventEx([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChangedEvent(propertyName);
            RaisePropertyChangedEvent(propertyName + "Brush");
            RaisePropertyChangedEvent(propertyName + "FontWeight");
        }

        /// <summary>
        ///     RaisePropertyChangedEvent(null) creates an infinite loop, so we list the separate properties here.
        ///     FIXME: there _must_ be a better way to do this?
        /// </summary>
        private void RaiseAllPropertiesChangedEvent()
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChangedEvent(nameof(RoomType));
                RaisePropertyChangedEvent(nameof(RoomConstant));
                RaisePropertyChangedEvent(nameof(CoolingSetpoint));
                RaisePropertyChangedEvent(nameof(HeatingSetpoint));
                RaisePropertyChangedEvent(nameof(CoolingSetback));
                RaisePropertyChangedEvent(nameof(HeatingSetback));
                RaisePropertyChangedEvent(nameof(RunAdaptiveComfort));
                RaisePropertyChangedEvent(nameof(FloorArea));
                RaisePropertyChangedEvent(nameof(EnvelopeArea));
                RaisePropertyChangedEvent(nameof(GlazingRatio));

                var properties = new[]
                {
                    "UValueTransparent",
                    "GValue",
                    "GValueTotal",
                    "ShadingSetpoint",
                    "WindowFrameReduction",
                    "AirChangeRate",
                    "Infiltration", 
                    "HeatRecovery", 
                    "OccupantLoads", 
                    "LightingLoads",
                    "EquipmentLoads", 
                    "OccupantYearlyHours",
                    "LightingYearlyHours",
                    "EquipmentYearlyHours",
                    "TransparentCost",
                    "TransparentEmissions",

                    "UValueFloors",
                    "UValueRoofs",
                    "UValueWalls",
                    "CostFloors",
                    "CostRoofs",
                    "CostWalls",
                    "EmissionsFloors",
                    "EmissionsRoofs",
                    "EmissionsWalls"
                };
                foreach (var property in properties)
                {
                    RaisePropertyChangedEvent(property);
                    RaisePropertyChangedEvent(property + "Brush");
                    RaisePropertyChangedEvent(property + "FontWeight");
                }
            }
        }

        #region areas

        // NOTE: the funny syntax (x?.a ?? y.b) returns x.a, unless x is null, then it returns y.b
        // it works like this: (x?.a) is x.a if x != null, else null. (A ?? B) is A if A != null, else B
        // I'm using this to enable creating a BuildingInputSate with zone == null for testing purposes.
        public string ZoneWallArea => $"{_zone?.WallArea ?? _siaRoom.EnvelopeArea:0.00}";
        public string ZoneFloorArea => $"{_zone?.FloorArea ?? _siaRoom.FloorArea:0.00}";
        public string ZoneRoofArea => $"{_zone?.RoofArea ?? _siaRoom.EnvelopeArea:0.00}";
        public string ZoneWindowArea => $"{_zone?.WindowArea ?? _siaRoom.EnvelopeArea:0.00}";

        #endregion areas

        #region comboboxes

        public IEnumerable<string> BuildingUseTypes =>
            _editable ? Sia2024Record.BuildingUseTypes() : new List<string> {"<Custom>"};

        public IEnumerable<string> RoomTypes =>
            _editable ? Sia2024Record.RoomTypes(BuildingUseType) : new List<string> {RoomType};

        public IEnumerable<string> Qualities => _editable ? Sia2024Record.Qualities() : new List<string> {"<Custom>"};

        public string Quality
        {
            get => _siaRoom.Quality;
            set
            {
                _siaRoom = Sia2024Record.Lookup(BuildingUseType, RoomType, value) as Sia2024RecordEx;
                RaisePropertyChangedEvent();
                RaiseAllPropertiesChangedEvent();
            }
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public string BuildingUseType
        {
            get => _siaRoom.BuildingUseType;
            set
            {
                var roomType = Sia2024Record.RoomTypes(value).First();
                _siaRoom = Sia2024Record.Lookup(value, roomType, Quality) as Sia2024RecordEx;
                RaisePropertyChangedEvent();
                RaisePropertyChangedEvent("RoomType");
                RaisePropertyChangedEvent("RoomTypes");
                RaiseAllPropertiesChangedEvent();
            }
        }

        public string RoomType
        {
            get => _siaRoom.RoomType;
            set
            {
                _siaRoom = Sia2024Record.Lookup(BuildingUseType, value, Quality) as Sia2024RecordEx;
                RaisePropertyChangedEvent();
                RaiseAllPropertiesChangedEvent();
            }
        }

        #region zone properties

        public bool RunAdaptiveComfort
        {
            get => _zone.RunAdaptiveComfort;
            set
            {
                _zone.RunAdaptiveComfort = value;
                RaisePropertyChangedEvent();
            }
        }
        #endregion

        #endregion comboboxes

        #region sia2024 properties

        public string RoomConstant
        {
            get => $"{_siaRoom.RoomConstant:0.00}";
            set
            {
                try
                {
                    _siaRoom.RoomConstant = double.Parse(value);
                }
                catch (Exception)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string CoolingSetpoint
        {
            get => $"{_siaRoom.CoolingSetpoint:0.00}";
            set
            {
                try
                {
                    _siaRoom.CoolingSetpoint = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string HeatingSetpoint
        {
            get => $"{_siaRoom.HeatingSetpoint:0.00}";
            set
            {
                try
                {
                    _siaRoom.HeatingSetpoint = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string CoolingSetback
        {
            get => $"{_siaRoom.CoolingSetback:0.00}";
            set
            {
                try
                {
                    _siaRoom.CoolingSetback = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string HeatingSetback
        {
            get => $"{_siaRoom.HeatingSetback:0.00}";
            set
            {
                try
                {
                    _siaRoom.HeatingSetback = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string FloorArea
        {
            get => $"{_siaRoom.FloorArea:0.00}";
            set
            {
                try
                {
                    _siaRoom.FloorArea = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string EnvelopeArea
        {
            get => $"{_siaRoom.EnvelopeArea:0.00}";
            set
            {
                try
                {
                    _siaRoom.EnvelopeArea = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string GlazingRatio
        {
            get => $"{_siaRoom.GlazingRatio:0.00}";
            set
            {
                try
                {
                    _siaRoom.GlazingRatio = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string UValueFloors
        {
            get => $"{_siaRoom.UValueFloors:0.00}";
            set
            {
                try
                {
                    _siaRoom.UValueFloors = double.Parse(value);
                }
                catch (FormatException)
                {
                    // don't update the value                    
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string UValueRoofs
        {
            get => $"{_siaRoom.UValueRoofs:0.00}";
            set
            {
                try
                {
                    _siaRoom.UValueRoofs = double.Parse(value);
                }
                catch (FormatException)
                {
                    // don't update the value                    
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string UValueWalls
        {
            get => $"{_siaRoom.UValueWalls:0.00}";
            set
            {
                try
                {
                    _siaRoom.UValueWalls = double.Parse(value);
                }
                catch (FormatException)
                {
                    // don't update the value                    
                }

                RaisePropertyChangedEventEx();
            }
        }


        public string UValueTransparent
        {
            get => $"{_siaRoom.UValueTransparent:0.00}";
            set
            {
                try
                {
                    _siaRoom.UValueTransparent = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string GValue
        {
            get => $"{_siaRoom.GValue:0.00}";
            set
            {
                try
                {
                    _siaRoom.GValue = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string GValueTotal
        {
            get => $"{_siaRoom.GValueTotal:0.00}";
            set
            {
                try
                {
                    _siaRoom.GValueTotal = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string ShadingSetpoint
        {
            get => $"{_siaRoom.ShadingSetpoint:0.00}";
            set
            {
                try
                {
                    _siaRoom.ShadingSetpoint = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string WindowFrameReduction
        {
            get => $"{_siaRoom.WindowFrameReduction:0.00}";
            set
            {
                try
                {
                    _siaRoom.WindowFrameReduction = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string AirChangeRate
        {
            get => $"{_siaRoom.AirChangeRate:0.00}";
            set
            {
                try
                {
                    _siaRoom.AirChangeRate = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string Infiltration
        {
            get => $"{_siaRoom.Infiltration:0.00}";
            set
            {
                try
                {
                    _siaRoom.Infiltration = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string HeatRecovery
        {
            get => $"{_siaRoom.HeatRecovery:0.00}";
            set
            {
                try
                {
                    _siaRoom.HeatRecovery = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string OccupantLoads
        {
            get => $"{_siaRoom.OccupantLoads:0.00}";
            set
            {
                try
                {
                    _siaRoom.OccupantLoads = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string LightingLoads
        {
            get => $"{_siaRoom.LightingLoads:0.00}";
            set
            {
                try
                {
                    _siaRoom.LightingLoads = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string EquipmentLoads
        {
            get => $"{_siaRoom.EquipmentLoads:0.00}";
            set
            {
                try
                {
                    _siaRoom.EquipmentLoads = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string OccupantYearlyHours
        {
            get => $"{_siaRoom.OccupantYearlyHours:0.00}";
            set
            {
                try
                {
                    _siaRoom.OccupantYearlyHours = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string LightingYearlyHours
        {
            get => $"{_siaRoom.LightingYearlyHours:0.00}";
            set
            {
                try
                {
                    _siaRoom.LightingYearlyHours = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string EquipmentYearlyHours
        {
            get => $"{_siaRoom.EquipmentYearlyHours:0.00}";
            set
            {
                try
                {
                    _siaRoom.EquipmentYearlyHours = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string CostFloors
        {
            get => $"{_siaRoom.CostFloors:0.00}";
            set
            {
                try
                {
                    _siaRoom.CostFloors = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string CostRoofs
        {
            get => $"{_siaRoom.CostRoofs:0.00}";
            set
            {
                try
                {
                    _siaRoom.CostRoofs = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string CostWalls
        {
            get => $"{_siaRoom.CostWalls:0.00}";
            set
            {
                try
                {
                    _siaRoom.CostWalls = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string TransparentCost
        {
            get => $"{_siaRoom.TransparentCost:0.00}";
            set
            {
                try
                {
                    _siaRoom.TransparentCost = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string EmissionsFloors
        {
            get => $"{_siaRoom.EmissionsFloors:0.00}";
            set
            {
                try
                {
                    _siaRoom.EmissionsFloors = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string EmissionsRoofs
        {
            get => $"{_siaRoom.EmissionsRoofs:0.00}";
            set
            {
                try
                {
                    _siaRoom.EmissionsRoofs = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string EmissionsWalls
        {
            get => $"{_siaRoom.EmissionsWalls:0.00}";
            set
            {
                try
                {
                    _siaRoom.EmissionsWalls = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        public string TransparentEmissions
        {
            get => $"{_siaRoom.TransparentEmissions:0.00}";
            set
            {
                try
                {
                    _siaRoom.TransparentEmissions = double.Parse(value);
                }
                catch (FormatException)
                {
                }

                RaisePropertyChangedEventEx();
            }
        }

        #endregion sia2024 properties



        #region colors

        private readonly Brush _normalBrush = new SolidColorBrush(Colors.Black);
        private readonly Brush _modifiedBrush = new SolidColorBrush(Colors.ForestGreen);

        private bool AreEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.001;
        }

        private bool ModifiedField([CallerMemberName] string callerMemberName = null)
        {
            var member = callerMemberName.Replace("Brush", "").Replace("FontWeight", "");
            var fieldInfo = typeof(Sia2024RecordEx).GetField(member);
            return !AreEqual((double) fieldInfo.GetValue(_siaRoom),
                (double) fieldInfo.GetValue(Sia2024Record.Lookup(_siaRoom)));
        }

        private bool ModifiedProperty([CallerMemberName] string callerMemberName = null)
        {
            var member = callerMemberName.Replace("Brush", "").Replace("FontWeight", "");
            var propertyInfo = typeof(Sia2024RecordEx).GetProperty(member);
            return !AreEqual((double) propertyInfo.GetValue(_siaRoom),
                (double) propertyInfo.GetValue(Sia2024Record.Lookup(_siaRoom)));
        }

        public Brush RoomConstantBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush UValueFloorsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;
        public Brush UValueRoofsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;
        public Brush UValueWallsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;

        public Brush UValueTransparentBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush GValueBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush GValueTotalBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush ShadingSetpointBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush WindowFrameReductionBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush AirChangeRateBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush InfiltrationBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush HeatRecoveryBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush OccupantLoadsBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush LightingLoadsBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush EquipmentLoadsBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush OccupantYearlyHoursBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush LightingYearlyHoursBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush EquipmentYearlyHoursBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush CostFloorsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;
        public Brush CostRoofsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;
        public Brush CostWallsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;

        public Brush TransparentCostBrush => ModifiedField() ? _modifiedBrush : _normalBrush;
        public Brush EmissionsFloorsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;
        public Brush EmissionsRoofsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;
        public Brush EmissionsWallsBrush => ModifiedProperty() ? _modifiedBrush : _normalBrush;

        public Brush TransparentEmissionsBrush => ModifiedField() ? _modifiedBrush : _normalBrush;

        #endregion colors

        #region fontweights

        private readonly FontWeight _normalFontWeight = FontWeights.Normal;
        private readonly FontWeight _modifiedFontWeight = FontWeights.Bold;

        public FontWeight RoomConstantFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight UValueFloorsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight UValueRoofsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight UValueWallsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight UValueTransparentFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight GValueFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight GValueTotalFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight ShadingSetpointFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight WindowFrameReductionFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight AirChangeRateFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight InfiltrationFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight HeatRecoveryFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight OccupantLoadsFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight LightingLoadsFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight EquipmentLoadsFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight OccupantYearlyHoursFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight LightingYearlyHoursFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight EquipmentYearlyHoursFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight CostFloorsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight CostRoofsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight CostWallsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight TransparentCostFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight EmissionsFloorsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight EmissionsRoofsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight EmissionsWallsFontWeight => ModifiedProperty() ? _modifiedFontWeight : _normalFontWeight;
        public FontWeight TransparentEmissionsFontWeight => ModifiedField() ? _modifiedFontWeight : _normalFontWeight;

        #endregion fontweights
    }
}