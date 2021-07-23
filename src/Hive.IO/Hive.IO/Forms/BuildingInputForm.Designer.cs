using System.Windows.Forms;
using Hive.IO.Building;

namespace Hive.IO.Forms
{
    partial class BuildingInputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSIA = new System.Windows.Forms.TabPage();
            this.tableLayoutSiaMain = new System.Windows.Forms.TableLayoutPanel();
            this.txtRoomCapacitance = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.cboBuildingConstruction = new System.Windows.Forms.ComboBox();
            this.label43 = new System.Windows.Forms.Label();
            this.panelSiaProperties = new System.Windows.Forms.Panel();
            this.tableLayoutSiaProperties = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtFloorArea = new System.Windows.Forms.TextBox();
            this.txtWallArea = new System.Windows.Forms.TextBox();
            this.txtWinArea = new System.Windows.Forms.TextBox();
            this.txtRoofArea = new System.Windows.Forms.TextBox();
            this.txtHeatingSetPoint = new System.Windows.Forms.TextBox();
            this.txtCoolingSetPoint = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.txtHeatingSetback = new System.Windows.Forms.TextBox();
            this.txtCoolingSetback = new System.Windows.Forms.TextBox();
            this.checkBoxAdaptiveComfort = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBuildingUseType = new System.Windows.Forms.ComboBox();
            this.cboRoomType = new System.Windows.Forms.ComboBox();
            this.cboBuildingQuality = new System.Windows.Forms.ComboBox();
            this.tabENV = new System.Windows.Forms.TabPage();
            this.tableLayoutEnvelopeMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutEnvelopeRoof = new System.Windows.Forms.TableLayoutPanel();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.cboRoofTemplate = new System.Windows.Forms.ComboBox();
            this.txtRoofUValue = new System.Windows.Forms.TextBox();
            this.txtRoofEmissions = new System.Windows.Forms.TextBox();
            this.txtRoofCost = new System.Windows.Forms.TextBox();
            this.tableLayoutEnvelopeWindow = new System.Windows.Forms.TableLayoutPanel();
            this.txtWindowShadingSetpoint = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txtWindowGValueTotal = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.txtWindowCost = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.cboWindowTemplate = new System.Windows.Forms.ComboBox();
            this.txtWindowUValue = new System.Windows.Forms.TextBox();
            this.txtWindowGValue = new System.Windows.Forms.TextBox();
            this.txtWindowEmissions = new System.Windows.Forms.TextBox();
            this.tableLayoutEnvelopeFloor = new System.Windows.Forms.TableLayoutPanel();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.cboFloorTemplate = new System.Windows.Forms.ComboBox();
            this.txtFloorUValue = new System.Windows.Forms.TextBox();
            this.txtFloorEmissions = new System.Windows.Forms.TextBox();
            this.txtFloorCost = new System.Windows.Forms.TextBox();
            this.tableLayoutEnvelopeWall = new System.Windows.Forms.TableLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cboWallTemplate = new System.Windows.Forms.ComboBox();
            this.txtWallUValue = new System.Windows.Forms.TextBox();
            this.txtWallEmissions = new System.Windows.Forms.TextBox();
            this.txtWallCost = new System.Windows.Forms.TextBox();
            this.tabIG = new System.Windows.Forms.TabPage();
            this.tableLayoutInternalGains = new System.Windows.Forms.TableLayoutPanel();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.txtOccupantLoads = new System.Windows.Forms.TextBox();
            this.txtOccupantYearlyHours = new System.Windows.Forms.TextBox();
            this.txtLightingLoads = new System.Windows.Forms.TextBox();
            this.txtLightingYearlyHours = new System.Windows.Forms.TextBox();
            this.txtEquipmentLoads = new System.Windows.Forms.TextBox();
            this.txtEquipmentYearlyHours = new System.Windows.Forms.TextBox();
            this.tabVEN = new System.Windows.Forms.TabPage();
            this.tableLayoutVentilation = new System.Windows.Forms.TableLayoutPanel();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.txtAirChangeRate = new System.Windows.Forms.TextBox();
            this.txtInfiltration = new System.Windows.Forms.TextBox();
            this.txtHeatRecovery = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabSIA.SuspendLayout();
            this.tableLayoutSiaMain.SuspendLayout();
            this.panelSiaProperties.SuspendLayout();
            this.tableLayoutSiaProperties.SuspendLayout();
            this.tabENV.SuspendLayout();
            this.tableLayoutEnvelopeMain.SuspendLayout();
            this.tableLayoutEnvelopeRoof.SuspendLayout();
            this.tableLayoutEnvelopeWindow.SuspendLayout();
            this.tableLayoutEnvelopeFloor.SuspendLayout();
            this.tableLayoutEnvelopeWall.SuspendLayout();
            this.tabIG.SuspendLayout();
            this.tableLayoutInternalGains.SuspendLayout();
            this.tabVEN.SuspendLayout();
            this.tableLayoutVentilation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSIA);
            this.tabControl.Controls.Add(this.tabENV);
            this.tabControl.Controls.Add(this.tabIG);
            this.tabControl.Controls.Add(this.tabVEN);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(368, 442);
            this.tabControl.TabIndex = 0;
            // 
            // tabSIA
            // 
            this.tabSIA.Controls.Add(this.tableLayoutSiaMain);
            this.tabSIA.Location = new System.Drawing.Point(4, 22);
            this.tabSIA.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabSIA.Name = "tabSIA";
            this.tabSIA.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabSIA.Size = new System.Drawing.Size(360, 416);
            this.tabSIA.TabIndex = 0;
            this.tabSIA.Text = "SIA";
            this.tabSIA.UseVisualStyleBackColor = true;
            // 
            // tableLayoutSiaMain
            // 
            this.tableLayoutSiaMain.ColumnCount = 2;
            this.tableLayoutSiaMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSiaMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSiaMain.Controls.Add(this.txtRoomCapacitance, 1, 4);
            this.tableLayoutSiaMain.Controls.Add(this.label44, 0, 4);
            this.tableLayoutSiaMain.Controls.Add(this.cboBuildingConstruction, 1, 3);
            this.tableLayoutSiaMain.Controls.Add(this.label43, 0, 3);
            this.tableLayoutSiaMain.Controls.Add(this.panelSiaProperties, 0, 6);
            this.tableLayoutSiaMain.Controls.Add(this.label1, 0, 0);
            this.tableLayoutSiaMain.Controls.Add(this.label2, 0, 1);
            this.tableLayoutSiaMain.Controls.Add(this.label3, 0, 2);
            this.tableLayoutSiaMain.Controls.Add(this.cboBuildingUseType, 1, 0);
            this.tableLayoutSiaMain.Controls.Add(this.cboRoomType, 1, 1);
            this.tableLayoutSiaMain.Controls.Add(this.cboBuildingQuality, 1, 2);
            this.tableLayoutSiaMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSiaMain.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutSiaMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutSiaMain.Name = "tableLayoutSiaMain";
            this.tableLayoutSiaMain.Padding = new System.Windows.Forms.Padding(4, 8, 4, 4);
            this.tableLayoutSiaMain.RowCount = 8;
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutSiaMain.Size = new System.Drawing.Size(356, 412);
            this.tableLayoutSiaMain.TabIndex = 0;
            // 
            // txtRoomCapacitance
            // 
            this.txtRoomCapacitance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRoomCapacitance.Enabled = false;
            this.txtRoomCapacitance.Location = new System.Drawing.Point(180, 112);
            this.txtRoomCapacitance.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtRoomCapacitance.Name = "txtRoomCapacitance";
            this.txtRoomCapacitance.Size = new System.Drawing.Size(170, 20);
            this.txtRoomCapacitance.TabIndex = 21;
            this.txtRoomCapacitance.Tag = "RoomCapacitance";
            this.txtRoomCapacitance.TextChanged += new System.EventHandler(this.txtRoomCapacitance_TextChanged);
            // 
            // label44
            // 
            this.label44.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(6, 109);
            this.label44.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(139, 26);
            this.label44.TabIndex = 10;
            this.label44.Text = "Capacitance per Floor Area (W/m²K)";
            // 
            // cboBuildingConstruction
            // 
            this.cboBuildingConstruction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingConstruction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingConstruction.FormattingEnabled = true;
            this.cboBuildingConstruction.Location = new System.Drawing.Point(180, 85);
            this.cboBuildingConstruction.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboBuildingConstruction.Name = "cboBuildingConstruction";
            this.cboBuildingConstruction.Size = new System.Drawing.Size(170, 21);
            this.cboBuildingConstruction.TabIndex = 9;
            this.cboBuildingConstruction.SelectedIndexChanged += new System.EventHandler(this.cboBuildingConstruction_SelectedIndexChanged);
            // 
            // label43
            // 
            this.label43.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(6, 89);
            this.label43.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(128, 13);
            this.label43.TabIndex = 8;
            this.label43.Text = "Building construction type";
            // 
            // panelSiaProperties
            // 
            this.panelSiaProperties.AutoSize = true;
            this.panelSiaProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutSiaMain.SetColumnSpan(this.panelSiaProperties, 2);
            this.panelSiaProperties.Controls.Add(this.tableLayoutSiaProperties);
            this.panelSiaProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSiaProperties.Location = new System.Drawing.Point(6, 247);
            this.panelSiaProperties.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelSiaProperties.Name = "panelSiaProperties";
            this.panelSiaProperties.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelSiaProperties.Size = new System.Drawing.Size(344, 143);
            this.panelSiaProperties.TabIndex = 0;
            // 
            // tableLayoutSiaProperties
            // 
            this.tableLayoutSiaProperties.AutoSize = true;
            this.tableLayoutSiaProperties.ColumnCount = 4;
            this.tableLayoutSiaProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutSiaProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutSiaProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutSiaProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutSiaProperties.Controls.Add(this.label5, 0, 0);
            this.tableLayoutSiaProperties.Controls.Add(this.label6, 2, 0);
            this.tableLayoutSiaProperties.Controls.Add(this.label7, 0, 1);
            this.tableLayoutSiaProperties.Controls.Add(this.label8, 0, 2);
            this.tableLayoutSiaProperties.Controls.Add(this.label9, 2, 1);
            this.tableLayoutSiaProperties.Controls.Add(this.label10, 2, 2);
            this.tableLayoutSiaProperties.Controls.Add(this.label11, 0, 3);
            this.tableLayoutSiaProperties.Controls.Add(this.label12, 0, 4);
            this.tableLayoutSiaProperties.Controls.Add(this.txtFloorArea, 1, 1);
            this.tableLayoutSiaProperties.Controls.Add(this.txtWallArea, 1, 2);
            this.tableLayoutSiaProperties.Controls.Add(this.txtWinArea, 1, 3);
            this.tableLayoutSiaProperties.Controls.Add(this.txtRoofArea, 1, 4);
            this.tableLayoutSiaProperties.Controls.Add(this.txtHeatingSetPoint, 3, 1);
            this.tableLayoutSiaProperties.Controls.Add(this.txtCoolingSetPoint, 3, 2);
            this.tableLayoutSiaProperties.Controls.Add(this.label41, 2, 4);
            this.tableLayoutSiaProperties.Controls.Add(this.label38, 2, 3);
            this.tableLayoutSiaProperties.Controls.Add(this.txtHeatingSetback, 3, 3);
            this.tableLayoutSiaProperties.Controls.Add(this.txtCoolingSetback, 3, 4);
            this.tableLayoutSiaProperties.Controls.Add(this.checkBoxAdaptiveComfort, 3, 0);
            this.tableLayoutSiaProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSiaProperties.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutSiaProperties.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutSiaProperties.Name = "tableLayoutSiaProperties";
            this.tableLayoutSiaProperties.RowCount = 5;
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutSiaProperties.Size = new System.Drawing.Size(334, 133);
            this.tableLayoutSiaProperties.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(2, 4);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Geometry";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(168, 4);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Set-points";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2, 28);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Floor Area (m²)";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2, 56);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Wall Area (m²)";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(168, 28);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Heating";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(168, 56);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Cooling";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(2, 84);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Win Area (m²)";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(2, 112);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Roof Area (m²)";
            // 
            // txtFloorArea
            // 
            this.txtFloorArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorArea.Enabled = false;
            this.txtFloorArea.Location = new System.Drawing.Point(85, 25);
            this.txtFloorArea.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtFloorArea.Name = "txtFloorArea";
            this.txtFloorArea.Size = new System.Drawing.Size(79, 20);
            this.txtFloorArea.TabIndex = 8;
            // 
            // txtWallArea
            // 
            this.txtWallArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallArea.Enabled = false;
            this.txtWallArea.Location = new System.Drawing.Point(85, 53);
            this.txtWallArea.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWallArea.Name = "txtWallArea";
            this.txtWallArea.Size = new System.Drawing.Size(79, 20);
            this.txtWallArea.TabIndex = 9;
            // 
            // txtWinArea
            // 
            this.txtWinArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWinArea.Enabled = false;
            this.txtWinArea.Location = new System.Drawing.Point(85, 81);
            this.txtWinArea.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWinArea.Name = "txtWinArea";
            this.txtWinArea.Size = new System.Drawing.Size(79, 20);
            this.txtWinArea.TabIndex = 10;
            // 
            // txtRoofArea
            // 
            this.txtRoofArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofArea.Enabled = false;
            this.txtRoofArea.Location = new System.Drawing.Point(85, 109);
            this.txtRoofArea.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtRoofArea.Name = "txtRoofArea";
            this.txtRoofArea.Size = new System.Drawing.Size(79, 20);
            this.txtRoofArea.TabIndex = 11;
            // 
            // txtHeatingSetPoint
            // 
            this.txtHeatingSetPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeatingSetPoint.Enabled = false;
            this.txtHeatingSetPoint.Location = new System.Drawing.Point(251, 25);
            this.txtHeatingSetPoint.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtHeatingSetPoint.Name = "txtHeatingSetPoint";
            this.txtHeatingSetPoint.Size = new System.Drawing.Size(81, 20);
            this.txtHeatingSetPoint.TabIndex = 12;
            this.txtHeatingSetPoint.Tag = "SetpointHeating";
            // 
            // txtCoolingSetPoint
            // 
            this.txtCoolingSetPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCoolingSetPoint.Enabled = false;
            this.txtCoolingSetPoint.Location = new System.Drawing.Point(251, 53);
            this.txtCoolingSetPoint.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtCoolingSetPoint.Name = "txtCoolingSetPoint";
            this.txtCoolingSetPoint.Size = new System.Drawing.Size(81, 20);
            this.txtCoolingSetPoint.TabIndex = 13;
            this.txtCoolingSetPoint.Tag = "SetpointCooling";
            // 
            // label41
            // 
            this.label41.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(168, 112);
            this.label41.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(71, 13);
            this.label41.TabIndex = 15;
            this.label41.Text = "Cool Setback";
            // 
            // label38
            // 
            this.label38.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(168, 84);
            this.label38.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(73, 13);
            this.label38.TabIndex = 16;
            this.label38.Text = "Heat Setback";
            // 
            // txtHeatingSetback
            // 
            this.txtHeatingSetback.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeatingSetback.Enabled = false;
            this.txtHeatingSetback.Location = new System.Drawing.Point(251, 81);
            this.txtHeatingSetback.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtHeatingSetback.Name = "txtHeatingSetback";
            this.txtHeatingSetback.Size = new System.Drawing.Size(81, 20);
            this.txtHeatingSetback.TabIndex = 17;
            this.txtHeatingSetback.Tag = "SetbackHeating";
            // 
            // txtCoolingSetback
            // 
            this.txtCoolingSetback.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCoolingSetback.Enabled = false;
            this.txtCoolingSetback.Location = new System.Drawing.Point(251, 109);
            this.txtCoolingSetback.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtCoolingSetback.Name = "txtCoolingSetback";
            this.txtCoolingSetback.Size = new System.Drawing.Size(81, 20);
            this.txtCoolingSetback.TabIndex = 18;
            this.txtCoolingSetback.Tag = "SetbackCooling";
            // 
            // checkBoxAdaptiveComfort
            // 
            this.checkBoxAdaptiveComfort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxAdaptiveComfort.AutoSize = true;
            this.checkBoxAdaptiveComfort.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxAdaptiveComfort.Checked = true;
            this.checkBoxAdaptiveComfort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAdaptiveComfort.Location = new System.Drawing.Point(251, 2);
            this.checkBoxAdaptiveComfort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.checkBoxAdaptiveComfort.Name = "checkBoxAdaptiveComfort";
            this.checkBoxAdaptiveComfort.Size = new System.Drawing.Size(74, 17);
            this.checkBoxAdaptiveComfort.TabIndex = 20;
            this.checkBoxAdaptiveComfort.Text = "Adaptive?";
            this.checkBoxAdaptiveComfort.UseVisualStyleBackColor = true;
            this.checkBoxAdaptiveComfort.CheckedChanged += new System.EventHandler(this.adaptiveComfortCheck_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Building use type";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Room type";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 64);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Building quality";
            // 
            // cboBuildingUseType
            // 
            this.cboBuildingUseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingUseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingUseType.FormattingEnabled = true;
            this.cboBuildingUseType.Location = new System.Drawing.Point(180, 10);
            this.cboBuildingUseType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboBuildingUseType.Name = "cboBuildingUseType";
            this.cboBuildingUseType.Size = new System.Drawing.Size(170, 21);
            this.cboBuildingUseType.TabIndex = 4;
            this.cboBuildingUseType.SelectedIndexChanged += new System.EventHandler(this.cboBuildingUseType_SelectedIndexChanged);
            // 
            // cboRoomType
            // 
            this.cboRoomType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRoomType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoomType.FormattingEnabled = true;
            this.cboRoomType.Location = new System.Drawing.Point(180, 35);
            this.cboRoomType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboRoomType.Name = "cboRoomType";
            this.cboRoomType.Size = new System.Drawing.Size(170, 21);
            this.cboRoomType.TabIndex = 5;
            this.cboRoomType.SelectedIndexChanged += new System.EventHandler(this.cboRoomType_SelectedIndexChanged);
            // 
            // cboBuildingQuality
            // 
            this.cboBuildingQuality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingQuality.FormattingEnabled = true;
            this.cboBuildingQuality.Location = new System.Drawing.Point(180, 60);
            this.cboBuildingQuality.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboBuildingQuality.Name = "cboBuildingQuality";
            this.cboBuildingQuality.Size = new System.Drawing.Size(170, 21);
            this.cboBuildingQuality.TabIndex = 6;
            this.cboBuildingQuality.SelectedIndexChanged += new System.EventHandler(this.cboBuildingQuality_SelectedIndexChanged);
            // 
            // tabENV
            // 
            this.tabENV.Controls.Add(this.tableLayoutEnvelopeMain);
            this.tabENV.Location = new System.Drawing.Point(4, 22);
            this.tabENV.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabENV.Name = "tabENV";
            this.tabENV.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabENV.Size = new System.Drawing.Size(360, 416);
            this.tabENV.TabIndex = 1;
            this.tabENV.Text = "Envelope";
            this.tabENV.UseVisualStyleBackColor = true;
            // 
            // tableLayoutEnvelopeMain
            // 
            this.tableLayoutEnvelopeMain.ColumnCount = 2;
            this.tableLayoutEnvelopeMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutEnvelopeMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutEnvelopeMain.Controls.Add(this.tableLayoutEnvelopeRoof, 1, 1);
            this.tableLayoutEnvelopeMain.Controls.Add(this.tableLayoutEnvelopeWindow, 0, 1);
            this.tableLayoutEnvelopeMain.Controls.Add(this.tableLayoutEnvelopeFloor, 1, 0);
            this.tableLayoutEnvelopeMain.Controls.Add(this.tableLayoutEnvelopeWall, 0, 0);
            this.tableLayoutEnvelopeMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeMain.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutEnvelopeMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutEnvelopeMain.Name = "tableLayoutEnvelopeMain";
            this.tableLayoutEnvelopeMain.Padding = new System.Windows.Forms.Padding(4, 8, 4, 4);
            this.tableLayoutEnvelopeMain.RowCount = 2;
            this.tableLayoutEnvelopeMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutEnvelopeMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutEnvelopeMain.Size = new System.Drawing.Size(356, 412);
            this.tableLayoutEnvelopeMain.TabIndex = 0;
            // 
            // tableLayoutEnvelopeRoof
            // 
            this.tableLayoutEnvelopeRoof.ColumnCount = 2;
            this.tableLayoutEnvelopeRoof.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeRoof.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label25, 0, 0);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label26, 0, 2);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label27, 0, 4);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label28, 0, 5);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.cboRoofTemplate, 0, 1);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.txtRoofUValue, 1, 2);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.txtRoofEmissions, 1, 4);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.txtRoofCost, 1, 5);
            this.tableLayoutEnvelopeRoof.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeRoof.Location = new System.Drawing.Point(180, 210);
            this.tableLayoutEnvelopeRoof.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutEnvelopeRoof.Name = "tableLayoutEnvelopeRoof";
            this.tableLayoutEnvelopeRoof.RowCount = 7;
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.Size = new System.Drawing.Size(170, 196);
            this.tableLayoutEnvelopeRoof.TabIndex = 3;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(2, 4);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(34, 13);
            this.label25.TabIndex = 0;
            this.label25.Text = "Roof";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(2, 54);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(87, 13);
            this.label26.TabIndex = 1;
            this.label26.Text = "U-value (W/m²K)";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(2, 82);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(111, 13);
            this.label27.TabIndex = 2;
            this.label27.Text = "Emissions (kgCO2/m²)";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(2, 110);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(74, 13);
            this.label28.TabIndex = 3;
            this.label28.Text = "Cost (CHF/m²)";
            // 
            // cboRoofTemplate
            // 
            this.cboRoofTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeRoof.SetColumnSpan(this.cboRoofTemplate, 2);
            this.cboRoofTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoofTemplate.FormattingEnabled = true;
            this.cboRoofTemplate.Location = new System.Drawing.Point(2, 25);
            this.cboRoofTemplate.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.cboRoofTemplate.Name = "cboRoofTemplate";
            this.cboRoofTemplate.Size = new System.Drawing.Size(166, 21);
            this.cboRoofTemplate.TabIndex = 4;
            this.cboRoofTemplate.SelectedIndexChanged += new System.EventHandler(this.cboRoofTemplate_SelectedIndexChanged);
            // 
            // txtRoofUValue
            // 
            this.txtRoofUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofUValue.Location = new System.Drawing.Point(121, 54);
            this.txtRoofUValue.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtRoofUValue.Name = "txtRoofUValue";
            this.txtRoofUValue.Size = new System.Drawing.Size(47, 20);
            this.txtRoofUValue.TabIndex = 5;
            this.txtRoofUValue.Tag = "UValueRoofs";
            this.txtRoofUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtRoofEmissions
            // 
            this.txtRoofEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofEmissions.Location = new System.Drawing.Point(121, 82);
            this.txtRoofEmissions.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtRoofEmissions.Name = "txtRoofEmissions";
            this.txtRoofEmissions.Size = new System.Drawing.Size(47, 20);
            this.txtRoofEmissions.TabIndex = 6;
            this.txtRoofEmissions.Tag = "EmissionsRoofs";
            this.txtRoofEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtRoofCost
            // 
            this.txtRoofCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofCost.Location = new System.Drawing.Point(121, 110);
            this.txtRoofCost.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtRoofCost.Name = "txtRoofCost";
            this.txtRoofCost.Size = new System.Drawing.Size(47, 20);
            this.txtRoofCost.TabIndex = 7;
            this.txtRoofCost.Tag = "CostRoofs";
            this.txtRoofCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tableLayoutEnvelopeWindow
            // 
            this.tableLayoutEnvelopeWindow.ColumnCount = 2;
            this.tableLayoutEnvelopeWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeWindow.Controls.Add(this.txtWindowShadingSetpoint, 1, 7);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label31, 0, 7);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.txtWindowGValueTotal, 1, 6);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label30, 0, 6);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label29, 0, 5);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.txtWindowCost, 1, 5);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label21, 0, 0);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label22, 0, 2);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label23, 0, 3);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.label24, 0, 4);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.cboWindowTemplate, 0, 1);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.txtWindowUValue, 1, 2);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.txtWindowGValue, 1, 3);
            this.tableLayoutEnvelopeWindow.Controls.Add(this.txtWindowEmissions, 1, 4);
            this.tableLayoutEnvelopeWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeWindow.Location = new System.Drawing.Point(6, 210);
            this.tableLayoutEnvelopeWindow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutEnvelopeWindow.Name = "tableLayoutEnvelopeWindow";
            this.tableLayoutEnvelopeWindow.RowCount = 9;
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutEnvelopeWindow.Size = new System.Drawing.Size(170, 196);
            this.tableLayoutEnvelopeWindow.TabIndex = 2;
            // 
            // txtWindowShadingSetpoint
            // 
            this.txtWindowShadingSetpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowShadingSetpoint.Location = new System.Drawing.Point(121, 197);
            this.txtWindowShadingSetpoint.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWindowShadingSetpoint.Name = "txtWindowShadingSetpoint";
            this.txtWindowShadingSetpoint.Size = new System.Drawing.Size(47, 20);
            this.txtWindowShadingSetpoint.TabIndex = 13;
            this.txtWindowShadingSetpoint.Tag = "ShadingSetpoint";
            this.txtWindowShadingSetpoint.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(2, 194);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(89, 26);
            this.label31.TabIndex = 12;
            this.label31.Text = "Shading setpoint (W/m²)";
            // 
            // txtWindowGValueTotal
            // 
            this.txtWindowGValueTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowGValueTotal.Location = new System.Drawing.Point(121, 166);
            this.txtWindowGValueTotal.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWindowGValueTotal.Name = "txtWindowGValueTotal";
            this.txtWindowGValueTotal.Size = new System.Drawing.Size(47, 20);
            this.txtWindowGValueTotal.TabIndex = 11;
            this.txtWindowGValueTotal.Tag = "GValueTotal";
            this.txtWindowGValueTotal.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(2, 166);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(77, 13);
            this.label30.TabIndex = 10;
            this.label30.Text = "g-value total (-)";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(2, 138);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(74, 13);
            this.label29.TabIndex = 8;
            this.label29.Text = "Cost (CHF/m²)";
            // 
            // txtWindowCost
            // 
            this.txtWindowCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowCost.Location = new System.Drawing.Point(121, 138);
            this.txtWindowCost.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWindowCost.Name = "txtWindowCost";
            this.txtWindowCost.Size = new System.Drawing.Size(47, 20);
            this.txtWindowCost.TabIndex = 9;
            this.txtWindowCost.Tag = "TransparentCost";
            this.txtWindowCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(2, 4);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(52, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Window";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(2, 54);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(87, 13);
            this.label22.TabIndex = 1;
            this.label22.Text = "U-value (W/m²K)";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(2, 82);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(54, 13);
            this.label23.TabIndex = 2;
            this.label23.Text = "g-value (-)";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(2, 110);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(111, 13);
            this.label24.TabIndex = 3;
            this.label24.Text = "Emissions (kgCO2/m²)";
            // 
            // cboWindowTemplate
            // 
            this.cboWindowTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeWindow.SetColumnSpan(this.cboWindowTemplate, 2);
            this.cboWindowTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWindowTemplate.FormattingEnabled = true;
            this.cboWindowTemplate.Location = new System.Drawing.Point(2, 25);
            this.cboWindowTemplate.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.cboWindowTemplate.Name = "cboWindowTemplate";
            this.cboWindowTemplate.Size = new System.Drawing.Size(166, 21);
            this.cboWindowTemplate.TabIndex = 4;
            this.cboWindowTemplate.SelectedIndexChanged += new System.EventHandler(this.cboWindowTemplate_SelectedIndexChanged);
            // 
            // txtWindowUValue
            // 
            this.txtWindowUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowUValue.Location = new System.Drawing.Point(121, 54);
            this.txtWindowUValue.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWindowUValue.Name = "txtWindowUValue";
            this.txtWindowUValue.Size = new System.Drawing.Size(47, 20);
            this.txtWindowUValue.TabIndex = 5;
            this.txtWindowUValue.Tag = "UValueTransparent";
            this.txtWindowUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWindowGValue
            // 
            this.txtWindowGValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowGValue.Location = new System.Drawing.Point(121, 82);
            this.txtWindowGValue.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWindowGValue.Name = "txtWindowGValue";
            this.txtWindowGValue.Size = new System.Drawing.Size(47, 20);
            this.txtWindowGValue.TabIndex = 6;
            this.txtWindowGValue.Tag = "GValue";
            this.txtWindowGValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWindowEmissions
            // 
            this.txtWindowEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowEmissions.Location = new System.Drawing.Point(121, 110);
            this.txtWindowEmissions.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWindowEmissions.Name = "txtWindowEmissions";
            this.txtWindowEmissions.Size = new System.Drawing.Size(47, 20);
            this.txtWindowEmissions.TabIndex = 7;
            this.txtWindowEmissions.Tag = "TransparentEmissions";
            this.txtWindowEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tableLayoutEnvelopeFloor
            // 
            this.tableLayoutEnvelopeFloor.ColumnCount = 2;
            this.tableLayoutEnvelopeFloor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeFloor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label17, 0, 0);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label18, 0, 2);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label19, 0, 4);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label20, 0, 5);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.cboFloorTemplate, 0, 1);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.txtFloorUValue, 1, 2);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.txtFloorEmissions, 1, 4);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.txtFloorCost, 1, 5);
            this.tableLayoutEnvelopeFloor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeFloor.Location = new System.Drawing.Point(180, 10);
            this.tableLayoutEnvelopeFloor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutEnvelopeFloor.Name = "tableLayoutEnvelopeFloor";
            this.tableLayoutEnvelopeFloor.RowCount = 7;
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutEnvelopeFloor.Size = new System.Drawing.Size(170, 196);
            this.tableLayoutEnvelopeFloor.TabIndex = 1;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(2, 4);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(35, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Floor";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(2, 54);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(87, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "U-value (W/m²K)";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(2, 82);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(111, 13);
            this.label19.TabIndex = 2;
            this.label19.Text = "Emissions (kgCO2/m²)";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(2, 110);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(74, 13);
            this.label20.TabIndex = 3;
            this.label20.Text = "Cost (CHF/m²)";
            // 
            // cboFloorTemplate
            // 
            this.cboFloorTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeFloor.SetColumnSpan(this.cboFloorTemplate, 2);
            this.cboFloorTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloorTemplate.FormattingEnabled = true;
            this.cboFloorTemplate.Location = new System.Drawing.Point(2, 25);
            this.cboFloorTemplate.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.cboFloorTemplate.Name = "cboFloorTemplate";
            this.cboFloorTemplate.Size = new System.Drawing.Size(166, 21);
            this.cboFloorTemplate.TabIndex = 4;
            this.cboFloorTemplate.SelectedIndexChanged += new System.EventHandler(this.cboFloorTemplate_SelectedIndexChanged);
            // 
            // txtFloorUValue
            // 
            this.txtFloorUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorUValue.Location = new System.Drawing.Point(121, 54);
            this.txtFloorUValue.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtFloorUValue.Name = "txtFloorUValue";
            this.txtFloorUValue.Size = new System.Drawing.Size(47, 20);
            this.txtFloorUValue.TabIndex = 5;
            this.txtFloorUValue.Tag = "UValueFloors";
            this.txtFloorUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtFloorEmissions
            // 
            this.txtFloorEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorEmissions.Location = new System.Drawing.Point(121, 82);
            this.txtFloorEmissions.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtFloorEmissions.Name = "txtFloorEmissions";
            this.txtFloorEmissions.Size = new System.Drawing.Size(47, 20);
            this.txtFloorEmissions.TabIndex = 6;
            this.txtFloorEmissions.Tag = "EmissionsFloors";
            this.txtFloorEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtFloorCost
            // 
            this.txtFloorCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorCost.Location = new System.Drawing.Point(121, 110);
            this.txtFloorCost.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtFloorCost.Name = "txtFloorCost";
            this.txtFloorCost.Size = new System.Drawing.Size(47, 20);
            this.txtFloorCost.TabIndex = 7;
            this.txtFloorCost.Tag = "CostFloors";
            this.txtFloorCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tableLayoutEnvelopeWall
            // 
            this.tableLayoutEnvelopeWall.ColumnCount = 2;
            this.tableLayoutEnvelopeWall.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeWall.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeWall.Controls.Add(this.label13, 0, 0);
            this.tableLayoutEnvelopeWall.Controls.Add(this.label14, 0, 2);
            this.tableLayoutEnvelopeWall.Controls.Add(this.label15, 0, 4);
            this.tableLayoutEnvelopeWall.Controls.Add(this.label16, 0, 5);
            this.tableLayoutEnvelopeWall.Controls.Add(this.cboWallTemplate, 0, 1);
            this.tableLayoutEnvelopeWall.Controls.Add(this.txtWallUValue, 1, 2);
            this.tableLayoutEnvelopeWall.Controls.Add(this.txtWallEmissions, 1, 4);
            this.tableLayoutEnvelopeWall.Controls.Add(this.txtWallCost, 1, 5);
            this.tableLayoutEnvelopeWall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeWall.Location = new System.Drawing.Point(6, 10);
            this.tableLayoutEnvelopeWall.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutEnvelopeWall.Name = "tableLayoutEnvelopeWall";
            this.tableLayoutEnvelopeWall.RowCount = 8;
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutEnvelopeWall.Size = new System.Drawing.Size(170, 196);
            this.tableLayoutEnvelopeWall.TabIndex = 0;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(2, 4);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Wall";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(2, 54);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(87, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "U-value (W/m²K)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(2, 82);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(111, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Emissions (kgCO2/m²)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(2, 110);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(74, 13);
            this.label16.TabIndex = 3;
            this.label16.Text = "Cost (CHF/m²)";
            // 
            // cboWallTemplate
            // 
            this.cboWallTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeWall.SetColumnSpan(this.cboWallTemplate, 2);
            this.cboWallTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWallTemplate.FormattingEnabled = true;
            this.cboWallTemplate.Location = new System.Drawing.Point(2, 25);
            this.cboWallTemplate.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.cboWallTemplate.Name = "cboWallTemplate";
            this.cboWallTemplate.Size = new System.Drawing.Size(166, 21);
            this.cboWallTemplate.TabIndex = 4;
            this.cboWallTemplate.SelectedIndexChanged += new System.EventHandler(this.cboWallTemplate_SelectedIndexChanged);
            // 
            // txtWallUValue
            // 
            this.txtWallUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallUValue.Location = new System.Drawing.Point(121, 54);
            this.txtWallUValue.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWallUValue.Name = "txtWallUValue";
            this.txtWallUValue.Size = new System.Drawing.Size(47, 20);
            this.txtWallUValue.TabIndex = 5;
            this.txtWallUValue.Tag = "UValueWalls";
            this.txtWallUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWallEmissions
            // 
            this.txtWallEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallEmissions.Location = new System.Drawing.Point(121, 82);
            this.txtWallEmissions.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWallEmissions.Name = "txtWallEmissions";
            this.txtWallEmissions.Size = new System.Drawing.Size(47, 20);
            this.txtWallEmissions.TabIndex = 6;
            this.txtWallEmissions.Tag = "EmissionsWalls";
            this.txtWallEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWallCost
            // 
            this.txtWallCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallCost.Location = new System.Drawing.Point(121, 110);
            this.txtWallCost.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.txtWallCost.Name = "txtWallCost";
            this.txtWallCost.Size = new System.Drawing.Size(47, 20);
            this.txtWallCost.TabIndex = 7;
            this.txtWallCost.Tag = "CostWalls";
            this.txtWallCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tabIG
            // 
            this.tabIG.Controls.Add(this.tableLayoutInternalGains);
            this.tabIG.Location = new System.Drawing.Point(4, 22);
            this.tabIG.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabIG.Name = "tabIG";
            this.tabIG.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabIG.Size = new System.Drawing.Size(360, 416);
            this.tabIG.TabIndex = 2;
            this.tabIG.Text = "Internal Gains";
            this.tabIG.UseVisualStyleBackColor = true;
            // 
            // tableLayoutInternalGains
            // 
            this.tableLayoutInternalGains.ColumnCount = 2;
            this.tableLayoutInternalGains.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutInternalGains.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutInternalGains.Controls.Add(this.label32, 1, 0);
            this.tableLayoutInternalGains.Controls.Add(this.label33, 0, 0);
            this.tableLayoutInternalGains.Controls.Add(this.label34, 0, 2);
            this.tableLayoutInternalGains.Controls.Add(this.label35, 1, 2);
            this.tableLayoutInternalGains.Controls.Add(this.label36, 0, 4);
            this.tableLayoutInternalGains.Controls.Add(this.label37, 1, 4);
            this.tableLayoutInternalGains.Controls.Add(this.txtOccupantLoads, 0, 1);
            this.tableLayoutInternalGains.Controls.Add(this.txtOccupantYearlyHours, 1, 1);
            this.tableLayoutInternalGains.Controls.Add(this.txtLightingLoads, 0, 3);
            this.tableLayoutInternalGains.Controls.Add(this.txtLightingYearlyHours, 1, 3);
            this.tableLayoutInternalGains.Controls.Add(this.txtEquipmentLoads, 0, 5);
            this.tableLayoutInternalGains.Controls.Add(this.txtEquipmentYearlyHours, 1, 5);
            this.tableLayoutInternalGains.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutInternalGains.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutInternalGains.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutInternalGains.Name = "tableLayoutInternalGains";
            this.tableLayoutInternalGains.Padding = new System.Windows.Forms.Padding(4, 8, 4, 4);
            this.tableLayoutInternalGains.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutInternalGains.RowCount = 7;
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.Size = new System.Drawing.Size(360, 416);
            this.tableLayoutInternalGains.TabIndex = 0;
            // 
            // label32
            // 
            this.label32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(182, 12);
            this.label32.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(172, 13);
            this.label32.TabIndex = 2;
            this.label32.Text = "People load hours (h)";
            // 
            // label33
            // 
            this.label33.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 12);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(172, 13);
            this.label33.TabIndex = 1;
            this.label33.Text = "People heat (W/m²)";
            // 
            // label34
            // 
            this.label34.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 61);
            this.label34.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(172, 13);
            this.label34.TabIndex = 3;
            this.label34.Text = "Light heat (W/m²)";
            // 
            // label35
            // 
            this.label35.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(182, 61);
            this.label35.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(172, 13);
            this.label35.TabIndex = 4;
            this.label35.Text = "Light load hours (h)";
            // 
            // label36
            // 
            this.label36.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(6, 110);
            this.label36.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(172, 13);
            this.label36.TabIndex = 5;
            this.label36.Text = "Equipment heat (W/m²)";
            // 
            // label37
            // 
            this.label37.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(182, 110);
            this.label37.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(172, 13);
            this.label37.TabIndex = 6;
            this.label37.Text = "Equipment load hours";
            // 
            // txtOccupantLoads
            // 
            this.txtOccupantLoads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOccupantLoads.Location = new System.Drawing.Point(19, 33);
            this.txtOccupantLoads.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtOccupantLoads.Name = "txtOccupantLoads";
            this.txtOccupantLoads.Size = new System.Drawing.Size(146, 20);
            this.txtOccupantLoads.TabIndex = 7;
            this.txtOccupantLoads.Tag = "OccupantLoads";
            this.txtOccupantLoads.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtOccupantYearlyHours
            // 
            this.txtOccupantYearlyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOccupantYearlyHours.Location = new System.Drawing.Point(195, 33);
            this.txtOccupantYearlyHours.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtOccupantYearlyHours.Name = "txtOccupantYearlyHours";
            this.txtOccupantYearlyHours.Size = new System.Drawing.Size(146, 20);
            this.txtOccupantYearlyHours.TabIndex = 8;
            this.txtOccupantYearlyHours.Tag = "OccupantYearlyHours";
            this.txtOccupantYearlyHours.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtLightingLoads
            // 
            this.txtLightingLoads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLightingLoads.Location = new System.Drawing.Point(19, 82);
            this.txtLightingLoads.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtLightingLoads.Name = "txtLightingLoads";
            this.txtLightingLoads.Size = new System.Drawing.Size(146, 20);
            this.txtLightingLoads.TabIndex = 9;
            this.txtLightingLoads.Tag = "LightingLoads";
            this.txtLightingLoads.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtLightingYearlyHours
            // 
            this.txtLightingYearlyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLightingYearlyHours.Location = new System.Drawing.Point(195, 82);
            this.txtLightingYearlyHours.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtLightingYearlyHours.Name = "txtLightingYearlyHours";
            this.txtLightingYearlyHours.Size = new System.Drawing.Size(146, 20);
            this.txtLightingYearlyHours.TabIndex = 10;
            this.txtLightingYearlyHours.Tag = "LightingYearlyHours";
            this.txtLightingYearlyHours.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtEquipmentLoads
            // 
            this.txtEquipmentLoads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEquipmentLoads.Location = new System.Drawing.Point(19, 131);
            this.txtEquipmentLoads.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtEquipmentLoads.Name = "txtEquipmentLoads";
            this.txtEquipmentLoads.Size = new System.Drawing.Size(146, 20);
            this.txtEquipmentLoads.TabIndex = 11;
            this.txtEquipmentLoads.Tag = "EquipmentLoads";
            this.txtEquipmentLoads.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtEquipmentYearlyHours
            // 
            this.txtEquipmentYearlyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEquipmentYearlyHours.Location = new System.Drawing.Point(195, 131);
            this.txtEquipmentYearlyHours.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtEquipmentYearlyHours.Name = "txtEquipmentYearlyHours";
            this.txtEquipmentYearlyHours.Size = new System.Drawing.Size(146, 20);
            this.txtEquipmentYearlyHours.TabIndex = 12;
            this.txtEquipmentYearlyHours.Tag = "EquipmentYearlyHours";
            this.txtEquipmentYearlyHours.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tabVEN
            // 
            this.tabVEN.Controls.Add(this.tableLayoutVentilation);
            this.tabVEN.Location = new System.Drawing.Point(4, 22);
            this.tabVEN.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabVEN.Name = "tabVEN";
            this.tabVEN.Size = new System.Drawing.Size(360, 416);
            this.tabVEN.TabIndex = 3;
            this.tabVEN.Text = "Ventilation";
            this.tabVEN.UseVisualStyleBackColor = true;
            // 
            // tableLayoutVentilation
            // 
            this.tableLayoutVentilation.ColumnCount = 2;
            this.tableLayoutVentilation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutVentilation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutVentilation.Controls.Add(this.label39, 0, 0);
            this.tableLayoutVentilation.Controls.Add(this.label40, 0, 2);
            this.tableLayoutVentilation.Controls.Add(this.label42, 0, 4);
            this.tableLayoutVentilation.Controls.Add(this.txtAirChangeRate, 0, 1);
            this.tableLayoutVentilation.Controls.Add(this.txtInfiltration, 0, 3);
            this.tableLayoutVentilation.Controls.Add(this.txtHeatRecovery, 0, 5);
            this.tableLayoutVentilation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutVentilation.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutVentilation.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutVentilation.Name = "tableLayoutVentilation";
            this.tableLayoutVentilation.Padding = new System.Windows.Forms.Padding(4, 8, 4, 4);
            this.tableLayoutVentilation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutVentilation.RowCount = 7;
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutVentilation.Size = new System.Drawing.Size(360, 416);
            this.tableLayoutVentilation.TabIndex = 1;
            // 
            // label39
            // 
            this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(6, 12);
            this.label39.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(172, 13);
            this.label39.TabIndex = 1;
            this.label39.Text = "Volumetric air flow (m³/m²h)";
            // 
            // label40
            // 
            this.label40.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(6, 61);
            this.label40.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(172, 13);
            this.label40.TabIndex = 3;
            this.label40.Text = "Infiltration (m³/m²h)";
            // 
            // label42
            // 
            this.label42.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(6, 110);
            this.label42.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(172, 13);
            this.label42.TabIndex = 5;
            this.label42.Text = "Heat recovery (-)";
            // 
            // txtAirChangeRate
            // 
            this.txtAirChangeRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAirChangeRate.Location = new System.Drawing.Point(19, 33);
            this.txtAirChangeRate.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtAirChangeRate.Name = "txtAirChangeRate";
            this.txtAirChangeRate.Size = new System.Drawing.Size(146, 20);
            this.txtAirChangeRate.TabIndex = 7;
            this.txtAirChangeRate.Tag = "AirChangeRate";
            this.txtAirChangeRate.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtInfiltration
            // 
            this.txtInfiltration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInfiltration.Location = new System.Drawing.Point(19, 82);
            this.txtInfiltration.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtInfiltration.Name = "txtInfiltration";
            this.txtInfiltration.Size = new System.Drawing.Size(146, 20);
            this.txtInfiltration.TabIndex = 9;
            this.txtInfiltration.Tag = "Infiltration";
            this.txtInfiltration.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtHeatRecovery
            // 
            this.txtHeatRecovery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeatRecovery.Location = new System.Drawing.Point(19, 131);
            this.txtHeatRecovery.Margin = new System.Windows.Forms.Padding(15, 4, 15, 4);
            this.txtHeatRecovery.Name = "txtHeatRecovery";
            this.txtHeatRecovery.Size = new System.Drawing.Size(146, 20);
            this.txtHeatRecovery.TabIndex = 11;
            this.txtHeatRecovery.Tag = "HeatRecovery";
            this.txtHeatRecovery.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // BuildingInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 442);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "BuildingInputForm";
            this.Text = "hizard | Building";
            this.Load += new System.EventHandler(this.BuildingInputForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabSIA.ResumeLayout(false);
            this.tableLayoutSiaMain.ResumeLayout(false);
            this.tableLayoutSiaMain.PerformLayout();
            this.panelSiaProperties.ResumeLayout(false);
            this.panelSiaProperties.PerformLayout();
            this.tableLayoutSiaProperties.ResumeLayout(false);
            this.tableLayoutSiaProperties.PerformLayout();
            this.tabENV.ResumeLayout(false);
            this.tableLayoutEnvelopeMain.ResumeLayout(false);
            this.tableLayoutEnvelopeRoof.ResumeLayout(false);
            this.tableLayoutEnvelopeRoof.PerformLayout();
            this.tableLayoutEnvelopeWindow.ResumeLayout(false);
            this.tableLayoutEnvelopeWindow.PerformLayout();
            this.tableLayoutEnvelopeFloor.ResumeLayout(false);
            this.tableLayoutEnvelopeFloor.PerformLayout();
            this.tableLayoutEnvelopeWall.ResumeLayout(false);
            this.tableLayoutEnvelopeWall.PerformLayout();
            this.tabIG.ResumeLayout(false);
            this.tableLayoutInternalGains.ResumeLayout(false);
            this.tableLayoutInternalGains.PerformLayout();
            this.tabVEN.ResumeLayout(false);
            this.tableLayoutVentilation.ResumeLayout(false);
            this.tableLayoutVentilation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSIA;
        private System.Windows.Forms.TabPage tabENV;
        private System.Windows.Forms.TabPage tabIG;
        private System.Windows.Forms.TabPage tabVEN;
        private TableLayoutPanel tableLayoutEnvelopeMain;
        private TableLayoutPanel tableLayoutEnvelopeWall;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private ComboBox cboWallTemplate;
        private TextBox txtWallUValue;
        private TextBox txtWallEmissions;
        private TextBox txtWallCost;
        private TableLayoutPanel tableLayoutEnvelopeRoof;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private ComboBox cboRoofTemplate;
        private TextBox txtRoofUValue;
        private TextBox txtRoofEmissions;
        private TextBox txtRoofCost;
        private TableLayoutPanel tableLayoutEnvelopeWindow;
        private TextBox txtWindowShadingSetpoint;
        private Label label31;
        private TextBox txtWindowGValueTotal;
        private Label label30;
        private Label label29;
        private TextBox txtWindowCost;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private ComboBox cboWindowTemplate;
        private TextBox txtWindowUValue;
        private TextBox txtWindowGValue;
        private TextBox txtWindowEmissions;
        private TableLayoutPanel tableLayoutEnvelopeFloor;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private ComboBox cboFloorTemplate;
        private TextBox txtFloorUValue;
        private TextBox txtFloorEmissions;
        private TextBox txtFloorCost;
        private TableLayoutPanel tableLayoutInternalGains;
        private Label label32;
        private Label label33;
        private Label label34;
        private Label label35;
        private Label label36;
        private Label label37;
        private TextBox txtOccupantLoads;
        private TextBox txtOccupantYearlyHours;
        private TextBox txtLightingLoads;
        private TextBox txtLightingYearlyHours;
        private TextBox txtEquipmentLoads;
        private TextBox txtEquipmentYearlyHours;
        private TableLayoutPanel tableLayoutVentilation;
        private Label label39;
        private Label label40;
        private Label label42;
        private TextBox txtAirChangeRate;
        private TextBox txtInfiltration;
        private TextBox txtHeatRecovery;
        private TableLayoutPanel tableLayoutSiaMain;
        private ComboBox cboBuildingConstruction;
        private Label label43;
        private Panel panelSiaProperties;
        private TableLayoutPanel tableLayoutSiaProperties;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private TextBox txtFloorArea;
        private TextBox txtWallArea;
        private TextBox txtWinArea;
        private TextBox txtRoofArea;
        private TextBox txtHeatingSetPoint;
        private TextBox txtCoolingSetPoint;
        private Label label41;
        private Label label38;
        private TextBox txtHeatingSetback;
        private TextBox txtCoolingSetback;
        private CheckBox checkBoxAdaptiveComfort;
        private Label label1;
        private Label label2;
        private Label label3;
        private ComboBox cboBuildingUseType;
        private ComboBox cboRoomType;
        private ComboBox cboBuildingQuality;
        private TextBox txtRoomCapacitance;
        private Label label44;
    }
}