using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Hive.IO.Building;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Capture the state of the BuildingInput form... allow binding controls to manipulate the _siaRoom...
    /// </summary>
    public class BuildingInputState : INotifyPropertyChanged
    {
        private Sia2024RecordEx _siaRoom;
        private bool _editable;


        public BuildingInputState(Sia2024RecordEx room, bool editable)
        {
            _siaRoom = room;
            _editable = editable;
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

        public bool IsEditable
        {
            get => _editable;
            set
            {
                _editable = value;
                RaisePropertyChangedEvent(null);
            }
        }

        public IEnumerable<string> BuildingUseTypes
        {
            get => _editable ? Sia2024Record.BuildingUseTypes() : new List<string> {"<Custom>"};
        }

        public IEnumerable<string> RoomTypes
        {
            get => _editable ? Sia2024Record.RoomTypes(BuildingUseType) : new List<string> {RoomType};
        }

        public IEnumerable<string> Qualities
        {
            get => _editable ? Sia2024Record.Qualities() : new List<string> {"<Custom>"};
        }

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

        public string RoomConstant
        {
            get => $"{_siaRoom.RoomConstant:0.00}";
            set
            {
                try
                {
                    _siaRoom.RoomConstant = double.Parse(value);
                }
                catch
                {
                }

                RaisePropertyChangedEvent();
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
                RaisePropertyChangedEvent();
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
                RaisePropertyChangedEvent();
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
                RaisePropertyChangedEvent();
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
                RaisePropertyChangedEvent();
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
                RaisePropertyChangedEvent();
            }
        }
        public string UValueOpaque
        {
            get => $"{_siaRoom.UValueOpaque:0.00}";
            set
            {
                try
                {
                    _siaRoom.UValueOpaque = double.Parse(value);
                }
                catch (FormatException e)
                {
                    // don't update the value                    
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
            }
        }
        public string OpaqueCost
        {
            get => $"{_siaRoom.OpaqueCost:0.00}";
            set
            {
                try
                {
                    _siaRoom.OpaqueCost = double.Parse(value);
                }
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
            }
        }
        public string OpaqueEmissions
        {
            get => $"{_siaRoom.OpaqueEmissions:0.00}";
            set
            {
                try
                {
                    _siaRoom.OpaqueEmissions = double.Parse(value);
                }
                catch
                {
                }
                RaisePropertyChangedEvent();
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
                catch
                {
                }
                RaisePropertyChangedEvent();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseAllPropertiesChangedEvent()
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChangedEvent(nameof(RoomType));
                RaisePropertyChangedEvent(nameof(RoomConstant));
                RaisePropertyChangedEvent(nameof(CoolingSetpoint));
                RaisePropertyChangedEvent(nameof(HeatingSetpoint));
                RaisePropertyChangedEvent(nameof(FloorArea));
                RaisePropertyChangedEvent(nameof(EnvelopeArea));
                RaisePropertyChangedEvent(nameof(GlazingRatio));
                RaisePropertyChangedEvent(nameof(UValueOpaque));
                RaisePropertyChangedEvent(nameof(UValueTransparent));
                RaisePropertyChangedEvent(nameof(GValue));
                RaisePropertyChangedEvent(nameof(WindowFrameReduction));
                RaisePropertyChangedEvent(nameof(AirChangeRate));
                RaisePropertyChangedEvent(nameof(Infiltration));
                RaisePropertyChangedEvent(nameof(HeatRecovery));
                RaisePropertyChangedEvent(nameof(OccupantLoads));
                RaisePropertyChangedEvent(nameof(LightingLoads));
                RaisePropertyChangedEvent(nameof(EquipmentLoads));
                RaisePropertyChangedEvent(nameof(OccupantYearlyHours));
                RaisePropertyChangedEvent(nameof(LightingYearlyHours));
                RaisePropertyChangedEvent(nameof(EquipmentYearlyHours));
                RaisePropertyChangedEvent(nameof(OpaqueCost));
                RaisePropertyChangedEvent(nameof(TransparentCost));
                RaisePropertyChangedEvent(nameof(OpaqueEmissions));
                RaisePropertyChangedEvent(nameof(TransparentEmissions));
            }
        }
    }
}