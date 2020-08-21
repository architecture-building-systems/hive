using System;
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
        }


        public BuildingInput(BuildingInputState state): this()
        {
            _state = state;
        }

        private void BuildingUseType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RoomType.ItemsSource = Sia2024Record.RoomTypes(BuildingUseType.SelectedItem as string);
            RoomType.SelectedIndex = 0;
            
            UpdateControls();
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
            UpdateControls();
        }

        /// <summary>
        /// Update the values in the controls based on the state
        /// </summary>
        /// <param name="siaRoom"></param>
        private void UpdateControls()
        {
            if (_state == null)
            {
                // happens, before Window is loaded
                return;
            }

            var useType = BuildingUseType.SelectedValue as string;
            var roomType = RoomType.SelectedValue as string;
            var quality = BuildingQuality.SelectedValue as string;

            var siaRoom = (Sia2024RecordEx)Sia2024Record.Lookup(useType, roomType, quality);
            if (siaRoom == null)
            {
                // still updating events...
                return;
            }
            _state.SiaRoom = siaRoom;

            BuildingUseType.Text = _state.SiaRoom.BuildingUseType;
            BuildingQuality.Text = _state.SiaRoom.Quality;
            RoomType.Text = _state.SiaRoom.RoomType;

            WallUValue.Text = $"{_state.SiaRoom.UValueOpaque:0.00}";
            WallCost.Text = $"{_state.SiaRoom.OpaqueCost:0.00}";
            WallEmissions.Text = $"{_state.SiaRoom.OpaqueEmissions:0.00}";
            WallTemplate.Text = $"(SIA) {_state.SiaRoom.RoomType}";

            RoofUValue.Text = $"{_state.SiaRoom.UValueOpaque:0.00}";
            RoofCost.Text = $"{_state.SiaRoom.OpaqueCost:0.00}"; ;
            RoofEmissions.Text = $"{_state.SiaRoom.OpaqueEmissions:0.00}";
            RoofTemplate.Text = $"(SIA) {_state.SiaRoom.RoomType}";

            FloorUValue.Text = $"{_state.SiaRoom.UValueOpaque:0.00}";
            FloorCost.Text = $"{_state.SiaRoom.OpaqueCost:0.00}";
            FloorEmissions.Text = $"{_state.SiaRoom.OpaqueEmissions:0.00}";
            FloorTemplate.Text = $"(SIA) {_state.SiaRoom.RoomType}";

            WindowUValue.Text = $"{_state.SiaRoom.UValueTransparent:0.00}";
            WindowGValue.Text = $"{_state.SiaRoom.GValue:0.00}";
            WindowCost.Text = $"{_state.SiaRoom.TransparentCost:0.00}";
            WindowEmissions.Text = $"{_state.SiaRoom.TransparentEmissions:0.00}";
            WindowTemplate.Text = $"(SIA) {_state.SiaRoom.RoomType}";
        }

        private void RoomType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls();
        }

        private void BuildingQuality_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateControls();
        }
    }
}
