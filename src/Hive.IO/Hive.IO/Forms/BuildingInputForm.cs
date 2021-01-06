using System;
using System.Windows.Forms;
using Hive.IO.Building;

namespace Hive.IO.Forms
{
    public partial class BuildingInputForm : Form
    {
        private BuildingInputState _state = new BuildingInputState(Sia2024Record.First(), null, true);

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
            UpdateSiaPropertiesPanel();
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
    }
}
