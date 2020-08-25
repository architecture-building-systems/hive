using System;
using System.Collections.Generic;
using System.Linq;
using Hive.IO.Building;
using Hive.IO.GHComponents;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Interaction logic for BuildingInput.xaml - tightly coupled to GhBuilding.
    /// </summary>
    public partial class BuildingInput
    {
        public BuildingInputState State { get; private set; }

        private BuildingInput()
        {
            State = new BuildingInputState(Sia2024Record.All().First() as Sia2024RecordEx, false);
            InitializeComponent();
        }


        public BuildingInput(BuildingInputState state) : this()
        {
            State.SiaRoom = state.SiaRoom;
            State.IsEditable = state.IsEditable;
        }


        /// <summary>
        /// Update the values in the controls based on the state
        /// </summary>
        /// <param name="siaRoom"></param>
        //private void UpdateControls(bool reloadRoom)
        //{
        //    if (_state == null)
        //    {
        //        // happens, before Window is loaded
        //        return;
        //    }

        //    var useType = BuildingUseType.SelectedValue as string;
        //    var roomType = RoomType.SelectedValue as string;
        //    var quality = BuildingQuality.SelectedValue as string;

        //    Sia2024RecordEx siaRoom;
        //    if (reloadRoom)
        //    {
        //        siaRoom = (Sia2024RecordEx)Sia2024Record.Lookup(useType, roomType, quality);
        //        if (siaRoom == null)
        //        {
        //            // still updating events...
        //            return;
        //        }
        //        _state.SiaRoom = siaRoom;
        //    }
        //    else
        //    {
        //        siaRoom = _state.SiaRoom;
        //    }
            
            

        //    BuildingUseType.Text = siaRoom.BuildingUseType;
        //    BuildingQuality.Text = siaRoom.Quality;
        //    RoomType.Text = siaRoom.RoomType;

        //    WallTemplate.Text = WallTemplate.SelectedItem as string;
        //    WallUValue.Text = $"{siaRoom.UValueOpaque:0.00}";
        //    WallCost.Text = $"{siaRoom.OpaqueCost:0.00}";
        //    WallEmissions.Text = $"{siaRoom.OpaqueEmissions:0.00}";
            
        //    RoofTemplate.Text = RoofTemplate.SelectedItem as string;
        //    RoofUValue.Text = $"{siaRoom.UValueOpaque:0.00}";
        //    RoofCost.Text = $"{siaRoom.OpaqueCost:0.00}"; ;
        //    RoofEmissions.Text = $"{siaRoom.OpaqueEmissions:0.00}";
            
        //    FloorTemplate.Text = FloorTemplate.SelectedItem as string;
        //    FloorUValue.Text = $"{siaRoom.UValueOpaque:0.00}";
        //    FloorCost.Text = $"{siaRoom.OpaqueCost:0.00}";
        //    FloorEmissions.Text = $"{siaRoom.OpaqueEmissions:0.00}";
            
        //    WindowTemplate.Text = WindowTemplate.SelectedItem as string;
        //    WindowUValue.Text = $"{siaRoom.UValueTransparent:0.00}";
        //    WindowGValue.Text = $"{siaRoom.GValue:0.00}";
        //    WindowCost.Text = $"{siaRoom.TransparentCost:0.00}";
        //    WindowEmissions.Text = $"{siaRoom.TransparentEmissions:0.00}";
        //}


        private void RoomType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void BuildingQuality_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void WallTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void FloorTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void WindowTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void RoofTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
    }
}
