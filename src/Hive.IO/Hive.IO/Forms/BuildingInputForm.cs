using System;
using System.Collections.Generic;
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
        private bool english = true;

        private Dictionary<string, string> ForwardDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> BackwardDictionary = new Dictionary<string, string>();

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

            //build Dictionary
            ForwardDictionary = getForwardDictionary();
            BackwardDictionary = ForwardDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

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

        private void buttonDE_Clicked(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            english = false;
            RenderState();
        }

        private void buttonEN_Clicked(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            english = true;
            RenderState();
        }

        private void UpdateSiaComboBoxes()
        {
            //Building Use Type
            cboBuildingUseType.Items.Clear();
            if (english) {
                cboBuildingUseType.Items.AddRange(Translate(State.BuildingUseTypes, ForwardDictionary).ToArray<object>());
                cboBuildingUseType.SelectedItem = ForwardDictionary[State.BuildingUseType];
            } else {
                cboBuildingUseType.Items.AddRange(State.BuildingUseTypes.ToArray<object>());
                cboBuildingUseType.SelectedItem = State.BuildingUseType;
            }
            cboBuildingUseType.Enabled = State.IsEditable;

            //Room Type
            cboRoomType.Items.Clear();
            if (english)
            {
                cboRoomType.Items.AddRange(Translate(State.RoomTypes, ForwardDictionary).ToArray<object>());
                cboRoomType.SelectedItem = ForwardDictionary[State.RoomType];
            }
            else
            {
                cboRoomType.Items.AddRange(State.RoomTypes.ToArray<object>());
                cboRoomType.SelectedItem = State.RoomType;
            }
            cboRoomType.Enabled = State.IsEditable;

            //Building Quality
            cboBuildingQuality.Items.Clear();
            if (english)
            {
                cboBuildingQuality.Items.AddRange(Translate(State.Qualities, ForwardDictionary).ToArray<object>());
                cboBuildingQuality.SelectedItem = ForwardDictionary[State.Quality];
            }
            else
            {
                cboBuildingQuality.Items.AddRange(State.Qualities.ToArray<object>());
                cboBuildingQuality.SelectedItem = State.Quality;
            }
            cboBuildingQuality.Enabled = State.IsEditable;

            //Building Construction
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

            State.BuildingUseType = english ? BackwardDictionary[cboBuildingUseType.SelectedItem as string] : cboBuildingUseType.SelectedItem as string;
            RenderState();
        }

        private void cboRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            State.RoomType = english ? BackwardDictionary[cboRoomType.SelectedItem as string] : cboRoomType.SelectedItem as string;
            RenderState();
        }

        private void cboBuildingQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rendering)
            {
                return;
            }

            State.Quality = english ? BackwardDictionary[cboBuildingQuality.SelectedItem as string] : cboBuildingQuality.SelectedItem as string;
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


        private IEnumerable<string> Translate(IEnumerable<string> _stringList, Dictionary<string, string> English)
        {
            var englishList = new List<string>();
            foreach(string german in _stringList)
            {
                englishList.Add(English[german]);
            }

            return englishList;
        }

        private Dictionary<string, string> getForwardDictionary()
        {
            Dictionary<string, string> fwd = new Dictionary<string, string>();

            fwd.Add("Wohnen", "Residential");
            fwd.Add("Hotel", "Hotel");
            fwd.Add("Buero", "Office");
            fwd.Add("Schule", "School");
            fwd.Add("Verkauf", "Retail");
            fwd.Add("Restaurant", "Restaurant");
            fwd.Add("Halle", "Hall");
            fwd.Add("Spital", "Hospital");
            fwd.Add("Werkstatt", "Workshop");
            fwd.Add("Lager", "Storage");
            fwd.Add("Sport", "Sports");
            fwd.Add("Diverses", "Miscellaneous");

            fwd.Add("1.1 Wohnen Mehrfamilienhaus", "1.1 Multi family home");
            fwd.Add("1.2 Wohnen Einfamilienhaus", "1.2 Single family home");

            fwd.Add("2.1 Hotelzimmer", "2.1 Hotel room");
            fwd.Add("2.2 Empfang, Lobby", "2.1 Reception, Lobby");

            fwd.Add("3.1 Einzel-, Gruppenbuero", "3.1 Single-, group office");
            fwd.Add("3.2 Grossraumbuero", "3.2 Open plan office");
            fwd.Add("3.3 Sitzungszimmer", "3.3 Meeting room");
            fwd.Add("3.4 Schalterhalle, Empfang", "3.4 Counter hall, Reception");

            fwd.Add("4.1 Schulzimmer", "4.1 Classroom");
            fwd.Add("4.2 Lehrerzimmer", "4.2 Staffroom");
            fwd.Add("4.3 Bibliothek", "4.3 Library");
            fwd.Add("4.4 Hoehrsaal", "4.4 Lecture hall");
            fwd.Add("4.5 Schulfachraum", "4.5 School subject room");

            fwd.Add("5.1 Lebensmittelverkauf", "5.1 Grocery sales");
            fwd.Add("5.2 Fachgeschaeft", "5.2 Specialty shop");
            fwd.Add("5.3 Verkauf Moebel, Bau, Garten", "5.3 Furniture, Hardware, Garden");

            fwd.Add("6.1 Restaurant", "6.1 Restaurant");
            fwd.Add("6.2 Selbstbedienungsrestaurant", "6.2 Self-service restaurant");
            fwd.Add("6.3 Kueche zu Restaurant", "6.3 Restaurant kitchen");
            fwd.Add("6.4 Kueche zu Selbstbedienungsrestaurant", "6.4 Self-service restaurant kitchen");

            fwd.Add("7.1 Vorstellungsraum", "7.1 Presentation room");
            fwd.Add("7.2 Mehrzweckhalle", "7.2 Multi-purpose hall");
            fwd.Add("7.3 Ausstellungshalle", "7.3 Exhibition hall");

            fwd.Add("8.1 Bettenzimmer", "8.1 Beds room");
            fwd.Add("8.2 Stationszimmer", "8.2 Ward room");
            fwd.Add("8.3 Behandlungsraum", "8.3 Treatment room");

            fwd.Add("9.1 Produktion (grobe Arbeit)", "9.1 Production (rough works)");
            fwd.Add("9.2 Produktion (feine Arbeit)", "9.2 Production (fine works)");
            fwd.Add("9.3 Laborraum", "9.3 Laboratory");

            fwd.Add("10.1 Lagerhalle", "10.1 Storage hall");

            fwd.Add("11.1 Turnhalle", "11.1 Sports hall");
            fwd.Add("11.2 Fitnessraum", "11.2 Gym");
            fwd.Add("11.3 Schwimmhalle", "11.3 Swimming pool");

            fwd.Add("12.1 Verkehrsflaeche", "12.1 Traffic area");
            fwd.Add("12.2 Verkehrsflaeche 24 h", "12.2 Traffic area 24 h");
            fwd.Add("12.3 Treppenhaus", "12.3 Stairwell");
            fwd.Add("12.4 Nebenraum", "12.4 Adjoining room");
            fwd.Add("12.5 Kueche, Teekueche", "12.5 Kitchen");
            fwd.Add("12.6 WC, Bad, Dusche", "12.5 Toilet, Shower, Bathroom");
            fwd.Add("12.7 WC", "12.7 Toilet");
            fwd.Add("12.8 Garderobe, Dusche", "12.8 Changing room, Shower");
            fwd.Add("12.9 Parkhaus", "12.9 Parking garage");
            fwd.Add("12.10 Wasch- und Trockenraum", "12.10 Laundry and drying room");
            fwd.Add("12.11 Kuehlraum", "12.11 Cold storage room");
            fwd.Add("12.12 Serverraum", "12.12 Server room");

            fwd.Add("Standardwert", "Standard value");
            fwd.Add("Bestand", "Existing building");
            fwd.Add("Zielwert", "Target value");

            return fwd;
        }

        #endregion ToolTips

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
