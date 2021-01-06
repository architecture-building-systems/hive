using System;
using System.Linq;
using System.Windows.Forms;
using Hive.IO.Building;

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
    }
}
