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
    /// <summary>
    /// Note on implementation: This form was originally a WPF Form using BuildingInputSate as a ViewModel.
    /// We ran into problems on some computers (see issue #500) and re-wrote the form in Windows.Forms. The ViewModel
    /// is still used, thought the I didn't hook up the PropertyChangedEvents - instead, we do this manually. So when
    /// viewing the ViewModel, don't worry too much about calls to RaisePropertyChanged* etc.
    /// </summary>
    public partial class BuildingInputForm : Form
    {
        private bool _rendering = false;

        public BuildingInputState State { get; private set; } = new BuildingInputState(Sia2024Record.First(), null, true);

        public DialogResult ShowDialog(BuildingInputState state)
        {
            State = state;
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
                UpdateInternalGainsTab();
                UpdateVentilationTab();
            }
            finally
            {
                _rendering = false;    
            }
        }

        private void UpdateSiaComboBoxes()
        {
            cboBuildingUseType.Items.Clear();
            cboBuildingUseType.Items.AddRange(State.BuildingUseTypes.ToArray<object>());
            cboBuildingUseType.SelectedItem = State.BuildingUseType;
            cboBuildingUseType.Enabled = State.IsEditable;

            cboRoomType.Items.Clear();
            cboRoomType.Items.AddRange(State.RoomTypes.ToArray<object>());
            cboRoomType.SelectedItem = State.RoomType;
            cboRoomType.Enabled = State.IsEditable;

            cboBuildingQuality.Items.Clear();
            cboBuildingQuality.Items.AddRange(State.Qualities.ToArray<object>());
            cboBuildingQuality.SelectedItem = State.Quality;
            cboBuildingQuality.Enabled = State.IsEditable;
        }

        private void UpdateSiaPropertiesPanel()
        {
            txtFloorArea.Text = State.FloorArea;
            txtWallArea.Text = State.ZoneWallArea;
            txtWinArea.Text = State.ZoneWindowArea;
            txtRoofArea.Text = State.ZoneRoofArea;

            txtHeatingSetPoint.Text = State.HeatingSetpoint;
            txtCoolingSetPoint.Text = State.CoolingSetpoint;
        }

        private void cboBuildingUseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            State.BuildingUseType = cboBuildingUseType.SelectedItem as string;
            RenderState();
        }

        private void cboRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            State.RoomType = cboRoomType.SelectedItem as string;
            RenderState();
        }

        private void cboBuildingQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            State.Quality = cboBuildingQuality.SelectedItem as string;
            RenderState();
        }

        private void UpdateEnvironmentTab()
        {
            cboWallTemplate.Enabled = false;
            UpdateTextBox(txtWallUValue);
            UpdateTextBox(txtWallEmissions);
            UpdateTextBox(txtWallCost);

            cboFloorTemplate.Enabled = false;
            UpdateTextBox(txtFloorUValue);
            UpdateTextBox(txtFloorEmissions);
            UpdateTextBox(txtFloorCost);

            cboWindowTemplate.Enabled = false;
            UpdateTextBox(txtWindowUValue);
            UpdateTextBox(txtWindowGValue);
            UpdateTextBox(txtWindowEmissions);
            UpdateTextBox(txtWindowCost);
            UpdateTextBox(txtWindowGValueTotal);
            UpdateTextBox(txtWindowShadingSetpoint);

            cboRoofTemplate.Enabled = false;
            UpdateTextBox(txtRoofUValue);
            UpdateTextBox(txtRoofEmissions);
            UpdateTextBox(txtRoofCost);
        }

        private void UpdateInternalGainsTab()
        {
            UpdateTextBox(txtOccupantLoads);
            UpdateTextBox(txtOccupantYearlyHours);
            UpdateTextBox(txtLightingLoads);
            UpdateTextBox(txtLightingYearlyHours);
            UpdateTextBox(txtEquipmentLoads);
            UpdateTextBox(txtEquipmentYearlyHours);
        }

        private void UpdateVentilationTab()
        {
            UpdateTextBox(txtAirChangeRate);
            UpdateTextBox(txtInfiltration);
            UpdateTextBox(txtHeatRecovery);
        }

        /// <summary>
        /// Use Reflection to update the textbox values based on the current state,
        /// including font weight and text color.
        ///
        /// Note that the ViewModel (BuildingInputState) was originally written for WPF, so we need to convert the
        /// FontWeight and SolidColorBrush to the Windows.Forms world.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="stateProperty"></param>
        private void UpdateTextBox(TextBox textBox)
        {
            var stateProperty = textBox.Tag.ToString();
            textBox.Text = State.GetType().GetProperty(stateProperty).GetValue(State) as string;
            textBox.Enabled = State.IsEditable;

            if (State.IsEditable)
            {
                var fontWeight = (FontWeight) State.GetType().GetProperty(stateProperty + "FontWeight").GetValue(State);
                textBox.Font = new Font(textBox.Font, fontWeight == FontWeights.Bold? FontStyle.Bold: FontStyle.Regular);

                var solidBrush = (SolidColorBrush) State.GetType().GetProperty(stateProperty + "Brush").GetValue(State);
                var foreColor = System.Drawing.Color.FromArgb(
                    solidBrush.Color.A, 
                    solidBrush.Color.R, 
                    solidBrush.Color.G,
                    solidBrush.Color.B);
                textBox.ForeColor = foreColor;
            }
        }

        /// <summary>
        /// The text in a textbox was changed - each textbox hooked up with this handler needs to have
        /// the "Tag" property set to the name of the corresponding BuildingInputSate property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            var textBox = (TextBox) sender;
            var stateProperty = textBox.Tag.ToString();

            State.GetType().GetProperty(stateProperty).SetValue(State, textBox.Text);

            RenderState();
        }
    }
}
