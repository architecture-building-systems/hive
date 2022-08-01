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

        public BuildingInputState State { get; private set; } = new BuildingInputState(Sia2024Record.First(), new Zone(), true);

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
            // Set ToolTips
            // Natural Ventilation
            hizardToolTip.SetToolTip(this.checkBoxNaturalVentilation, toolTipNaturalVentilationInfoMessage);
            hizardToolTip.SetToolTip(this.label45, toolTipNaturalVentilationInfoMessage);
            // Setbacks
            hizardToolTip.SetToolTip(this.txtHeatingSetback, toolTipSetbackInfoMessage);
            hizardToolTip.SetToolTip(this.label38, toolTipSetbackInfoMessage);
            hizardToolTip.SetToolTip(this.txtCoolingSetback, toolTipSetbackInfoMessage);
            hizardToolTip.SetToolTip(this.label41, toolTipSetbackInfoMessage);

            hizardToolTip.SetToolTip(this.txtFloorArea, toolTipFloorArea);
            hizardToolTip.SetToolTip(this.label7, toolTipFloorArea);

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

            cboBuildingConstruction.Items.Clear();
            cboBuildingConstruction.Items.AddRange(State.Constructions.ToArray<object>());
            cboBuildingConstruction.SelectedItem = State.Construction;
            cboBuildingConstruction.Enabled = State.IsEditable;

            UpdateTextBox(txtCapacitancePerFloorArea, editableOverride: false);
        }

        private void UpdateSiaPropertiesPanel()
        {
            txtFloorArea.Text = State.ZoneFloorArea;
            txtWallArea.Text = State.ZoneWallArea;
            txtWinArea.Text = State.ZoneWindowArea;
            txtRoofArea.Text = State.ZoneRoofArea;

            checkBoxAdaptiveComfort.Checked = State.RunAdaptiveComfort;

            UpdateTextBox(txtHeatingSetPoint, !State.RunAdaptiveComfort);
            UpdateTextBox(txtCoolingSetPoint, !State.RunAdaptiveComfort);
            UpdateTextBox(txtHeatingSetback, !State.RunAdaptiveComfort);
            UpdateTextBox(txtCoolingSetback, !State.RunAdaptiveComfort);
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

        private void cboBuildingConstruction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            State.Construction = cboBuildingConstruction.SelectedItem as string;

            cboWallTemplate.SelectedItem = State.Construction;
            cboFloorTemplate.SelectedItem = State.Construction;
            cboWindowTemplate.SelectedItem = State.Construction;
            cboRoofTemplate.SelectedItem = State.Construction;

            RenderState();
        }

        private void cboWallTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            //State.WallsConstruction = cboWallTemplate.SelectedItem as string; 
            //RenderState();
        }

        private void cboFloorTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            //State.FloorsConstruction = cboFloorTemplate.SelectedItem as string; 
            //RenderState();
        }

        private void cboWindowTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;
        }

        private void cboRoofTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            //State.RoofsConstruction = cboRoofTemplate.SelectedItem as string; 
            //RenderState();
        }

        private void UpdateEnvironmentTab()
        {
            cboWallTemplate.Enabled = false;
            cboWallTemplate.Items.Clear();
            cboWallTemplate.Items.AddRange(State.Constructions.ToArray<object>());
            cboWallTemplate.SelectedItem = State.Construction;
            UpdateTextBox(txtWallUValue);
            UpdateTextBox(txtWallEmissions);
            UpdateTextBox(txtWallCost);

            cboFloorTemplate.Enabled = false;
            cboFloorTemplate.Items.Clear();
            cboFloorTemplate.Items.AddRange(State.Constructions.ToArray<object>());
            cboFloorTemplate.SelectedItem = State.Construction;
            UpdateTextBox(txtFloorUValue);
            UpdateTextBox(txtFloorEmissions);
            UpdateTextBox(txtFloorCost);

            cboWindowTemplate.Enabled = false;
            cboWindowTemplate.Items.Clear();
            cboWindowTemplate.Items.AddRange(State.Constructions.ToArray<object>());
            cboWindowTemplate.SelectedItem = State.Construction;
            UpdateTextBox(txtWindowUValue);
            UpdateTextBox(txtWindowGValue);
            UpdateTextBox(txtWindowEmissions);
            UpdateTextBox(txtWindowCost);
            UpdateTextBox(txtWindowGValueTotal);
            UpdateTextBox(txtWindowShadingSetpoint);

            cboRoofTemplate.Enabled = false;
            cboRoofTemplate.Items.Clear();
            cboRoofTemplate.Items.AddRange(State.Constructions.ToArray<object>());
            cboRoofTemplate.SelectedItem = State.Construction;
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

            checkBoxNaturalVentilation.Checked = State.RunNaturalVentilation;
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
        private void UpdateTextBox(TextBox textBox, bool? editableOverride = null)
        {
            
            bool editable = State.IsEditable ? editableOverride ?? State.IsEditable : false;

            var stateProperty = textBox.Tag.ToString();
            textBox.Text = State.GetType().GetProperty(stateProperty).GetValue(State) as string;
            textBox.Enabled = editable;

            if (editable)
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

        // not pretty... TODO
        private bool ShouldSetpointsBeAdaptive(string room_type)
            => !new[] {
                    "9.3 Laborraum",
                    "11.2 Fitnessraum",
                    "11.3 Schwimmhalle",
                    "12.11 Kuehlraum",
                    "12.12 Serverraum"
            }.Contains(room_type);


        private void checkBoxAdaptiveComfort_CheckedChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            hizardToolTip.SetToolTip(checkBox,
                ShouldSetpointsBeAdaptive(State.RoomType)
                ? toolTipAdaptiveComfortInfoMessage
                : toolTipAdaptiveComfortWarningMessage(State.RoomType));

            if (checkBox.Checked)
            {
                txtHeatingSetPoint.Enabled = false;
                txtCoolingSetPoint.Enabled = false;
                txtHeatingSetback.Enabled = false;
                txtCoolingSetback.Enabled = false;
            }
            else
            {
                txtHeatingSetPoint.Enabled = true;
                txtCoolingSetPoint.Enabled = true;
                //txtHeatingSetback.Enabled = true;
                //txtCoolingSetback.Enabled = true;
            }

            State.RunAdaptiveComfort = checkBox.Checked;

            RenderState();
        }

        private void checkBoxNaturalVentilation_CheckedChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            var checkBox = (CheckBox)sender;
            State.RunNaturalVentilation = checkBox.Checked;

            RenderState();
        }
        
        #region ToolTips

        private ToolTip hizardToolTip = new ToolTip() { InitialDelay = 100 };

        private const string toolTipAdaptiveComfortInfoMessage = "Adaptive Comfort adjusts setpoints dynamically " +
                                                                 "based on ambient temperature \nand assumptions about metabolic rates and clothing factors. \n" +
                                                                 "!!! Please note: Activating this checkbox will deviate the loads calculation from the SIA 380 Norm. !!!";
        private string toolTipAdaptiveComfortWarningMessage(string roomType) =>
            toolTipAdaptiveComfortInfoMessage +
            "\n\nWARNING: Adaptive Comfort is likely not appropriate \n" +
            $"for room type {roomType}";

        private const string toolTipNaturalVentilationInfoMessage = "Natural Ventilation enables single sided natural ventilation" + "\n" +
            "based on simplified calculations of CIBSE AM10: Natural Ventilation in Non-Domestic Buildings. \n" +
            "!!! Please note: Activating this checkbox will deviate the loads calculation from the SIA 380 Norm. !!!";

        private const string toolTipSetbackInfoMessage = "Only used during hourly demand calculation, which is currently WIP";

        private const string toolTipFloorArea =
            "Note that this number refers to the gross area (Energiebezugsflaeche) measured from the hull geometry." + "\n " +
            "For loads calculation, this area will be reduced by 0.9, according to SIA 2024, E.3";

        private void textBoxSetback_MouseHover(object sender, EventArgs e)
        {
            hizardToolTip.Show(toolTipSetbackInfoMessage, (TextBox)sender);
        }

        #endregion ToolTips
    }
}
