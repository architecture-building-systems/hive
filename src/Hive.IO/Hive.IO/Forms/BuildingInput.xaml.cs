using System;
using System.Collections.Generic;
using Hive.IO.Building;
using Hive.IO.GHComponents;

namespace Hive.IO.Forms
{
    /// <summary>
    /// Interaction logic for BuildingInput.xaml - tightly coupled to GhBuilding.
    /// </summary>
    public partial class BuildingInput
    {
        private BuildingInputState _state;

        public BuildingInput()
        {
            InitializeComponent();

            BuildingUseType.ItemsSource = Sia2024Record.BuildingUseTypes();
            BuildingUseType.SelectedIndex = 0;

            RoomType.ItemsSource = Sia2024Record.RoomTypes(BuildingUseType.SelectedItem as string);
            RoomType.SelectedIndex = 0;

            BuildingQuality.ItemsSource = Sia2024Record.Qualities();
            BuildingQuality.SelectedIndex = 0;

            WallTemplate.ItemsSource = new List<string> { "<SIA 2024>" };
            RoofTemplate.ItemsSource = new List<string> { "<SIA 2024>" };
            FloorTemplate.ItemsSource = new List<string> { "<SIA 2024>" };
            WindowTemplate.ItemsSource = new List<string> { "<SIA 2024>" };

            WallTemplate.SelectedIndex = 0;
            RoofTemplate.SelectedIndex = 0;
            FloorTemplate.SelectedIndex = 0;
            WindowTemplate.SelectedIndex = 0;
        }


        public BuildingInput(BuildingInputState state) : this()
        {
            _state = state;
        }

        private void BuildingUseType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RoomType.ItemsSource = Sia2024Record.RoomTypes(BuildingUseType.SelectedItem as string);
            RoomType.SelectedIndex = 0;

            UpdateControls(true);
        }


        /// <summary>
        /// Load the data from state. This form can't be shown without the state property set...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_state == null)
            {
                throw new InvalidOperationException("Please set state.");
            }

            // read the stored values from the state and update the controls...
            UpdateControls(false);
        }

        /// <summary>
        /// Update the values in the controls based on the state
        /// </summary>
        /// <param name="siaRoom"></param>
        private void UpdateControls(bool reloadRoom)
        {
            if (_state == null)
            {
                // happens, before Window is loaded
                return;
            }

            var useType = BuildingUseType.SelectedValue as string;
            var roomType = RoomType.SelectedValue as string;
            var quality = BuildingQuality.SelectedValue as string;

            Sia2024RecordEx siaRoom;
            if (reloadRoom)
            {
                siaRoom = (Sia2024RecordEx)Sia2024Record.Lookup(useType, roomType, quality);
                if (siaRoom == null)
                {
                    // still updating events...
                    return;
                }
                _state.SiaRoom = siaRoom;
            }
            else
            {
                siaRoom = _state.SiaRoom;
            }
            
            

            BuildingUseType.Text = siaRoom.BuildingUseType;
            BuildingQuality.Text = siaRoom.Quality;
            RoomType.Text = siaRoom.RoomType;

            WallTemplate.Text = WallTemplate.SelectedItem as string;
            WallUValue.Text = $"{siaRoom.UValueOpaque:0.00}";
            WallCost.Text = $"{siaRoom.OpaqueCost:0.00}";
            WallEmissions.Text = $"{siaRoom.OpaqueEmissions:0.00}";
            
            RoofTemplate.Text = RoofTemplate.SelectedItem as string;
            RoofUValue.Text = $"{siaRoom.UValueOpaque:0.00}";
            RoofCost.Text = $"{siaRoom.OpaqueCost:0.00}"; ;
            RoofEmissions.Text = $"{siaRoom.OpaqueEmissions:0.00}";
            
            FloorTemplate.Text = FloorTemplate.SelectedItem as string;
            FloorUValue.Text = $"{siaRoom.UValueOpaque:0.00}";
            FloorCost.Text = $"{siaRoom.OpaqueCost:0.00}";
            FloorEmissions.Text = $"{siaRoom.OpaqueEmissions:0.00}";
            
            WindowTemplate.Text = WindowTemplate.SelectedItem as string;
            WindowUValue.Text = $"{siaRoom.UValueTransparent:0.00}";
            WindowGValue.Text = $"{siaRoom.GValue:0.00}";
            WindowCost.Text = $"{siaRoom.TransparentCost:0.00}";
            WindowEmissions.Text = $"{siaRoom.TransparentEmissions:0.00}";
        }

        private void RoomType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls(true);
        }

        private void BuildingQuality_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls(true);
        }

        private void WallTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls(false);
        }

        private void FloorTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls(false);
        }

        private void WindowTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls(false);
        }

        private void RoofTemplate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls(false);
        }
    }
}
