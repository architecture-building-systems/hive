using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Hive.IO.Building;
using FontStyle = System.Drawing.FontStyle;

namespace Hive.IO.Forms
{
    public partial class BuildingInputForm : Form
    {
        private BuildingInputState _state = new BuildingInputState(Sia2024Record.First(), null, true);
        private bool _rendering = false;

        public DialogResult ShowDialog(BuildingInputState state)
        {
            _state = state;
            return ShowDialog();
        }

        public BuildingInputForm()
        {
            InitializeComponent();
        }

        private void BuildingInputForm_Load(object sender, EventArgs e)
        {
            RenderState();
        }

        private void RenderState()
        {
            try
            {
                _rendering = true;
                UpdateSiaComboBoxes();
                UpdateSiaPropertiesPanel();
                UpdateEnvironmentTab();
            }
            finally
            {
                _rendering = false;    
            }
        }

        private void UpdateSiaComboBoxes()
        {
            cboBuildingUseType.Items.Clear();
            cboBuildingUseType.Items.AddRange(_state.BuildingUseTypes.ToArray<object>());
            cboBuildingUseType.SelectedItem = _state.BuildingUseType;

            cboRoomType.Items.Clear();
            cboRoomType.Items.AddRange(_state.RoomTypes.ToArray<object>());
            cboRoomType.SelectedItem = _state.RoomType;

            cboBuildingQuality.Items.Clear();
            cboBuildingQuality.Items.AddRange(_state.Qualities.ToArray<object>());
            cboBuildingQuality.SelectedItem = _state.Quality;
        }

        private void UpdateSiaPropertiesPanel()
        {
            txtFloorArea.Text = _state.FloorArea;
            txtWallArea.Text = _state.ZoneWallArea;
            txtWinArea.Text = _state.ZoneWindowArea;
            txtRoofArea.Text = _state.ZoneRoofArea;

            txtHeatingSetPoint.Text = _state.HeatingSetpoint;
            txtCoolingSetPoint.Text = _state.CoolingSetpoint;
        }

        private void cboBuildingUseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            _state.BuildingUseType = cboBuildingUseType.SelectedItem as string;
            RenderState();
        }

        private void cboRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            _state.RoomType = cboRoomType.SelectedItem as string;
            RenderState();
        }

        private void cboBuildingQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            _state.Quality = cboBuildingQuality.SelectedItem as string;
            RenderState();
        }

        private void UpdateEnvironmentTab()
        {
            cboWallTemplate.Enabled = false;
            UpdateTextBox(txtWallUValue, "UValueWalls");
            UpdateTextBox(txtWallEmissions, "EmissionsWalls");
            UpdateTextBox(txtWallCost, "CostWalls");

            cboFloorTemplate.Enabled = false;
            UpdateTextBox(txtFloorUValue, "UValueFloors");
            UpdateTextBox(txtFloorEmissions, "EmissionsFloors");
            UpdateTextBox(txtFloorCost, "CostFloors");

            cboWindowTemplate.Enabled = false;
            UpdateTextBox(txtWindowUValue, "UValueTransparent");
            UpdateTextBox(txtWindowGValue, "GValue");
            UpdateTextBox(txtWindowEmissions, "TransparentEmissions");
            UpdateTextBox(txtWindowCost, "TransparentCost");
            UpdateTextBox(txtWindowGValueTotal, "GValueTotal");
            UpdateTextBox(txtWindowShadingSetpoint, "ShadingSetpoint");

            cboRoofTemplate.Enabled = false;
            UpdateTextBox(txtRoofUValue, "UValueRoofs");
            UpdateTextBox(txtRoofEmissions, "EmissionsRoofs");
            UpdateTextBox(txtRoofCost, "CostRoofs");
        }

        /// <summary>
        /// Use Reflection to update the textbox values based on the current state,
        /// including font weight and text color.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="stateProperty"></param>
        private void UpdateTextBox(TextBox textBox, string stateProperty)
        {
            textBox.Text = _state.GetType().GetProperty(stateProperty).GetValue(_state) as string;

            var fontWeight = (FontWeight) _state.GetType().GetProperty(stateProperty + "FontWeight").GetValue(_state);
            textBox.Font = new Font(textBox.Font, fontWeight == FontWeights.Bold? FontStyle.Bold: FontStyle.Regular);

            var solidBrush = (SolidColorBrush) _state.GetType().GetProperty(stateProperty + "Brush").GetValue(_state);
            var foreColor = System.Drawing.Color.FromArgb(
                solidBrush.Color.A, 
                solidBrush.Color.R, 
                solidBrush.Color.G,
                solidBrush.Color.B);
            textBox.ForeColor = foreColor;
        }
    }
}
