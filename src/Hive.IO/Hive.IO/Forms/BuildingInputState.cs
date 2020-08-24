using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hive.IO.Building;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Capture the state of the BuildingInput form... allow binding controls to manipulate the _siaRoom...
    /// </summary>
    public class BuildingInputState: INotifyPropertyChanged
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
                RaisePropertyChangedEvent("SiaRoom");
            }
        }
        public bool IsEditable
        {
            get => _editable;
            set
            {
                _editable = value;
                RaisePropertyChangedEvent("IsEditable");
            }
        }

        public string RoomType
        {
            get => _siaRoom.RoomType;
            set
            {
                _siaRoom.RoomType = value;
                RaisePropertyChangedEvent("RoomType");
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
                
                RaisePropertyChangedEvent("RoomConstant");
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
                RaisePropertyChangedEvent("CoolingSetpoint");
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
                RaisePropertyChangedEvent("HeatingSetpoint");
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
                RaisePropertyChangedEvent("FloorArea");
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
                RaisePropertyChangedEvent("EnvelopeArea");
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
                RaisePropertyChangedEvent("GlazingRatio");
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
                RaisePropertyChangedEvent("UValueOpaque");
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
                RaisePropertyChangedEvent("UValueTransparent");
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
                RaisePropertyChangedEvent("GValue");
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
                RaisePropertyChangedEvent("WindowFrameReduction");
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
                RaisePropertyChangedEvent("AirChangeRate");
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
                RaisePropertyChangedEvent("Infiltration");
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
                RaisePropertyChangedEvent("HeatRecovery");
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
                RaisePropertyChangedEvent("OccupantLoads");
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
                RaisePropertyChangedEvent("LightingLoads");
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
                RaisePropertyChangedEvent("EquipmentLoads");
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
                RaisePropertyChangedEvent("OccupantYearlyHours");
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
                RaisePropertyChangedEvent("LightingYearlyHours");
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
                RaisePropertyChangedEvent("EquipmentYearlyHours");
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
                RaisePropertyChangedEvent("OpaqueCost");
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
                RaisePropertyChangedEvent("TransparentCost");
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
                RaisePropertyChangedEvent("OpaqueEmissions");
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
                RaisePropertyChangedEvent("TransparentEmissions");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}