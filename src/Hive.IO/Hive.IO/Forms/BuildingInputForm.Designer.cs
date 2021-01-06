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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBuildingUseType = new System.Windows.Forms.ComboBox();
            this.cboRoomType = new System.Windows.Forms.ComboBox();
            this.cboBuildingQuality = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabENV = new System.Windows.Forms.TabPage();
            this.tabIG = new System.Windows.Forms.TabPage();
            this.tabVEN = new System.Windows.Forms.TabPage();
            this.tableLayoutEnvelopeMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutEnvelopeWall = new System.Windows.Forms.TableLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cboWallTemplate = new System.Windows.Forms.ComboBox();
            this.txtWallUValue = new System.Windows.Forms.TextBox();
            this.txtWallEmissions = new System.Windows.Forms.TextBox();
            this.txtWallCost = new System.Windows.Forms.TextBox();
            this.tableLayoutEnvelopeFloor = new System.Windows.Forms.TableLayoutPanel();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.cboFloorTemplate = new System.Windows.Forms.ComboBox();
            this.txtFloorUValue = new System.Windows.Forms.TextBox();
            this.txtFloorEmissions = new System.Windows.Forms.TextBox();
            this.txtFloorCost = new System.Windows.Forms.TextBox();
            this.tableLayoutEnvelopeWindow = new System.Windows.Forms.TableLayoutPanel();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.cboWindowTemplate = new System.Windows.Forms.ComboBox();
            this.txtWindowUValue = new System.Windows.Forms.TextBox();
            this.txtWindowGValue = new System.Windows.Forms.TextBox();
            this.txtWindowEmissions = new System.Windows.Forms.TextBox();
            this.tableLayoutEnvelopeRoof = new System.Windows.Forms.TableLayoutPanel();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.cboRoofTemplate = new System.Windows.Forms.ComboBox();
            this.txtRoofUValue = new System.Windows.Forms.TextBox();
            this.txtRoofEmissions = new System.Windows.Forms.TextBox();
            this.txtRoofCost = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.txtWindowCost = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.txtWindowGValueTotal = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txtWindowShadingSetpoint = new System.Windows.Forms.TextBox();
            this.tableLayoutInternalGains = new System.Windows.Forms.TableLayoutPanel();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
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
            this.tabControl.SuspendLayout();
            this.tabSIA.SuspendLayout();
            this.tableLayoutSiaMain.SuspendLayout();
            this.panelSiaProperties.SuspendLayout();
            this.tableLayoutSiaProperties.SuspendLayout();
            this.tabENV.SuspendLayout();
            this.tabIG.SuspendLayout();
            this.tableLayoutEnvelopeMain.SuspendLayout();
            this.tableLayoutEnvelopeWall.SuspendLayout();
            this.tableLayoutEnvelopeFloor.SuspendLayout();
            this.tableLayoutEnvelopeWindow.SuspendLayout();
            this.tableLayoutEnvelopeRoof.SuspendLayout();
            this.tableLayoutInternalGains.SuspendLayout();
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
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1026, 1155);
            this.tabControl.TabIndex = 0;
            // 
            // tabSIA
            // 
            this.tabSIA.Controls.Add(this.tableLayoutSiaMain);
            this.tabSIA.Location = new System.Drawing.Point(10, 48);
            this.tabSIA.Name = "tabSIA";
            this.tabSIA.Padding = new System.Windows.Forms.Padding(3);
            this.tabSIA.Size = new System.Drawing.Size(1006, 1097);
            this.tabSIA.TabIndex = 0;
            this.tabSIA.Text = "SIA";
            this.tabSIA.UseVisualStyleBackColor = true;
            // 
            // tableLayoutSiaMain
            // 
            this.tableLayoutSiaMain.ColumnCount = 2;
            this.tableLayoutSiaMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSiaMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSiaMain.Controls.Add(this.panelSiaProperties, 0, 4);
            this.tableLayoutSiaMain.Controls.Add(this.label1, 0, 0);
            this.tableLayoutSiaMain.Controls.Add(this.label2, 0, 1);
            this.tableLayoutSiaMain.Controls.Add(this.label3, 0, 2);
            this.tableLayoutSiaMain.Controls.Add(this.cboBuildingUseType, 1, 0);
            this.tableLayoutSiaMain.Controls.Add(this.cboRoomType, 1, 1);
            this.tableLayoutSiaMain.Controls.Add(this.cboBuildingQuality, 1, 2);
            this.tableLayoutSiaMain.Controls.Add(this.label4, 0, 3);
            this.tableLayoutSiaMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSiaMain.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutSiaMain.Name = "tableLayoutSiaMain";
            this.tableLayoutSiaMain.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            this.tableLayoutSiaMain.RowCount = 5;
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.Size = new System.Drawing.Size(1000, 1091);
            this.tableLayoutSiaMain.TabIndex = 0;
            // 
            // panelSiaProperties
            // 
            this.panelSiaProperties.AutoSize = true;
            this.panelSiaProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutSiaMain.SetColumnSpan(this.panelSiaProperties, 2);
            this.panelSiaProperties.Controls.Add(this.tableLayoutSiaProperties);
            this.panelSiaProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSiaProperties.Location = new System.Drawing.Point(13, 772);
            this.panelSiaProperties.Name = "panelSiaProperties";
            this.panelSiaProperties.Padding = new System.Windows.Forms.Padding(10);
            this.panelSiaProperties.Size = new System.Drawing.Size(974, 306);
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
            this.tableLayoutSiaProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSiaProperties.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutSiaProperties.Name = "tableLayoutSiaProperties";
            this.tableLayoutSiaProperties.RowCount = 5;
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.Size = new System.Drawing.Size(952, 284);
            this.tableLayoutSiaProperties.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 10);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 32);
            this.label5.TabIndex = 0;
            this.label5.Text = "Geometry";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(479, 10);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 32);
            this.label6.TabIndex = 1;
            this.label6.Text = "Set-points";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(204, 32);
            this.label7.TabIndex = 2;
            this.label7.Text = "Floor Area (m²)";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(195, 32);
            this.label8.TabIndex = 3;
            this.label8.Text = "Wall Area (m²)";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(479, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 32);
            this.label9.TabIndex = 4;
            this.label9.Text = "Heating";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(479, 123);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 32);
            this.label10.TabIndex = 5;
            this.label10.Text = "Cooling";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 181);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(188, 32);
            this.label11.TabIndex = 6;
            this.label11.Text = "Win Area (m²)";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 239);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(199, 32);
            this.label12.TabIndex = 7;
            this.label12.Text = "Roof Area (m²)";
            // 
            // txtFloorArea
            // 
            this.txtFloorArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorArea.Enabled = false;
            this.txtFloorArea.Location = new System.Drawing.Point(241, 62);
            this.txtFloorArea.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtFloorArea.Name = "txtFloorArea";
            this.txtFloorArea.Size = new System.Drawing.Size(232, 38);
            this.txtFloorArea.TabIndex = 8;
            // 
            // txtWallArea
            // 
            this.txtWallArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallArea.Enabled = false;
            this.txtWallArea.Location = new System.Drawing.Point(241, 120);
            this.txtWallArea.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWallArea.Name = "txtWallArea";
            this.txtWallArea.Size = new System.Drawing.Size(232, 38);
            this.txtWallArea.TabIndex = 9;
            // 
            // txtWinArea
            // 
            this.txtWinArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWinArea.Enabled = false;
            this.txtWinArea.Location = new System.Drawing.Point(241, 178);
            this.txtWinArea.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWinArea.Name = "txtWinArea";
            this.txtWinArea.Size = new System.Drawing.Size(232, 38);
            this.txtWinArea.TabIndex = 10;
            // 
            // txtRoofArea
            // 
            this.txtRoofArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofArea.Enabled = false;
            this.txtRoofArea.Location = new System.Drawing.Point(241, 236);
            this.txtRoofArea.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtRoofArea.Name = "txtRoofArea";
            this.txtRoofArea.Size = new System.Drawing.Size(232, 38);
            this.txtRoofArea.TabIndex = 11;
            // 
            // txtHeatingSetPoint
            // 
            this.txtHeatingSetPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeatingSetPoint.Enabled = false;
            this.txtHeatingSetPoint.Location = new System.Drawing.Point(717, 62);
            this.txtHeatingSetPoint.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtHeatingSetPoint.Name = "txtHeatingSetPoint";
            this.txtHeatingSetPoint.Size = new System.Drawing.Size(232, 38);
            this.txtHeatingSetPoint.TabIndex = 12;
            // 
            // txtCoolingSetPoint
            // 
            this.txtCoolingSetPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCoolingSetPoint.Enabled = false;
            this.txtCoolingSetPoint.Location = new System.Drawing.Point(717, 120);
            this.txtCoolingSetPoint.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtCoolingSetPoint.Name = "txtCoolingSetPoint";
            this.txtCoolingSetPoint.Size = new System.Drawing.Size(232, 38);
            this.txtCoolingSetPoint.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Building use type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "Room type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 32);
            this.label3.TabIndex = 3;
            this.label3.Text = "Building quality";
            // 
            // cboBuildingUseType
            // 
            this.cboBuildingUseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingUseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingUseType.FormattingEnabled = true;
            this.cboBuildingUseType.Location = new System.Drawing.Point(503, 23);
            this.cboBuildingUseType.Name = "cboBuildingUseType";
            this.cboBuildingUseType.Size = new System.Drawing.Size(484, 39);
            this.cboBuildingUseType.TabIndex = 4;
            this.cboBuildingUseType.SelectedIndexChanged += new System.EventHandler(this.cboBuildingUseType_SelectedIndexChanged);
            // 
            // cboRoomType
            // 
            this.cboRoomType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRoomType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoomType.FormattingEnabled = true;
            this.cboRoomType.Location = new System.Drawing.Point(503, 68);
            this.cboRoomType.Name = "cboRoomType";
            this.cboRoomType.Size = new System.Drawing.Size(484, 39);
            this.cboRoomType.TabIndex = 5;
            this.cboRoomType.SelectedIndexChanged += new System.EventHandler(this.cboRoomType_SelectedIndexChanged);
            // 
            // cboBuildingQuality
            // 
            this.cboBuildingQuality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingQuality.FormattingEnabled = true;
            this.cboBuildingQuality.Location = new System.Drawing.Point(503, 113);
            this.cboBuildingQuality.Name = "cboBuildingQuality";
            this.cboBuildingQuality.Size = new System.Drawing.Size(484, 39);
            this.cboBuildingQuality.TabIndex = 6;
            this.cboBuildingQuality.SelectedIndexChanged += new System.EventHandler(this.cboBuildingQuality_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.tableLayoutSiaMain.SetColumnSpan(this.label4, 2);
            this.label4.Location = new System.Drawing.Point(13, 695);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.label4.Size = new System.Drawing.Size(974, 74);
            this.label4.TabIndex = 7;
            this.label4.Text = "Changing building quality and use types will impact the envelope, internal gains " +
    "and ventilation parameters";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tabENV
            // 
            this.tabENV.Controls.Add(this.tableLayoutEnvelopeMain);
            this.tabENV.Location = new System.Drawing.Point(10, 48);
            this.tabENV.Name = "tabENV";
            this.tabENV.Padding = new System.Windows.Forms.Padding(3);
            this.tabENV.Size = new System.Drawing.Size(1006, 1097);
            this.tabENV.TabIndex = 1;
            this.tabENV.Text = "Envelope";
            this.tabENV.UseVisualStyleBackColor = true;
            // 
            // tabIG
            // 
            this.tabIG.Controls.Add(this.tableLayoutInternalGains);
            this.tabIG.Location = new System.Drawing.Point(10, 48);
            this.tabIG.Name = "tabIG";
            this.tabIG.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabIG.Size = new System.Drawing.Size(1006, 1097);
            this.tabIG.TabIndex = 2;
            this.tabIG.Text = "Internal Gains";
            this.tabIG.UseVisualStyleBackColor = true;
            // 
            // tabVEN
            // 
            this.tabVEN.Location = new System.Drawing.Point(10, 48);
            this.tabVEN.Name = "tabVEN";
            this.tabVEN.Size = new System.Drawing.Size(1006, 1097);
            this.tabVEN.TabIndex = 3;
            this.tabVEN.Text = "Ventilation";
            this.tabVEN.UseVisualStyleBackColor = true;
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
            this.tableLayoutEnvelopeMain.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutEnvelopeMain.Name = "tableLayoutEnvelopeMain";
            this.tableLayoutEnvelopeMain.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            this.tableLayoutEnvelopeMain.RowCount = 2;
            this.tableLayoutEnvelopeMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutEnvelopeMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutEnvelopeMain.Size = new System.Drawing.Size(1000, 1091);
            this.tableLayoutEnvelopeMain.TabIndex = 0;
            // 
            // tableLayoutEnvelopeWall
            // 
            this.tableLayoutEnvelopeWall.ColumnCount = 2;
            this.tableLayoutEnvelopeWall.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeWall.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeWall.Controls.Add(this.label13, 0, 0);
            this.tableLayoutEnvelopeWall.Controls.Add(this.label14, 0, 2);
            this.tableLayoutEnvelopeWall.Controls.Add(this.label15, 0, 3);
            this.tableLayoutEnvelopeWall.Controls.Add(this.label16, 0, 4);
            this.tableLayoutEnvelopeWall.Controls.Add(this.cboWallTemplate, 0, 1);
            this.tableLayoutEnvelopeWall.Controls.Add(this.txtWallUValue, 1, 2);
            this.tableLayoutEnvelopeWall.Controls.Add(this.txtWallEmissions, 1, 3);
            this.tableLayoutEnvelopeWall.Controls.Add(this.txtWallCost, 1, 4);
            this.tableLayoutEnvelopeWall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeWall.Location = new System.Drawing.Point(13, 23);
            this.tableLayoutEnvelopeWall.Name = "tableLayoutEnvelopeWall";
            this.tableLayoutEnvelopeWall.RowCount = 8;
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeWall.Size = new System.Drawing.Size(484, 524);
            this.tableLayoutEnvelopeWall.TabIndex = 0;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(3, 10);
            this.label13.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(75, 32);
            this.label13.TabIndex = 0;
            this.label13.Text = "Wall";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 122);
            this.label14.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(223, 32);
            this.label14.TabIndex = 1;
            this.label14.Text = "U-value (W/m²K)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 180);
            this.label15.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(298, 32);
            this.label15.TabIndex = 2;
            this.label15.Text = "Emissions (kgCO2/m²)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 238);
            this.label16.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(195, 32);
            this.label16.TabIndex = 3;
            this.label16.Text = "Cost (CHF/m²)";
            // 
            // cboWallTemplate
            // 
            this.cboWallTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeWall.SetColumnSpan(this.cboWallTemplate, 2);
            this.cboWallTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWallTemplate.FormattingEnabled = true;
            this.cboWallTemplate.Location = new System.Drawing.Point(3, 62);
            this.cboWallTemplate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.cboWallTemplate.Name = "cboWallTemplate";
            this.cboWallTemplate.Size = new System.Drawing.Size(478, 39);
            this.cboWallTemplate.TabIndex = 4;
            // 
            // txtWallUValue
            // 
            this.txtWallUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallUValue.Location = new System.Drawing.Point(341, 122);
            this.txtWallUValue.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWallUValue.Name = "txtWallUValue";
            this.txtWallUValue.Size = new System.Drawing.Size(140, 38);
            this.txtWallUValue.TabIndex = 5;
            this.txtWallUValue.Tag = "UValueWalls";
            this.txtWallUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWallEmissions
            // 
            this.txtWallEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallEmissions.Location = new System.Drawing.Point(341, 180);
            this.txtWallEmissions.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWallEmissions.Name = "txtWallEmissions";
            this.txtWallEmissions.Size = new System.Drawing.Size(140, 38);
            this.txtWallEmissions.TabIndex = 6;
            this.txtWallEmissions.Tag = "EmissionsWalls";
            this.txtWallEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWallCost
            // 
            this.txtWallCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWallCost.Location = new System.Drawing.Point(341, 238);
            this.txtWallCost.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWallCost.Name = "txtWallCost";
            this.txtWallCost.Size = new System.Drawing.Size(140, 38);
            this.txtWallCost.TabIndex = 7;
            this.txtWallCost.Tag = "CostWalls";
            this.txtWallCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tableLayoutEnvelopeFloor
            // 
            this.tableLayoutEnvelopeFloor.ColumnCount = 2;
            this.tableLayoutEnvelopeFloor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeFloor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label17, 0, 0);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label18, 0, 2);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label19, 0, 3);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.label20, 0, 4);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.cboFloorTemplate, 0, 1);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.txtFloorUValue, 1, 2);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.txtFloorEmissions, 1, 3);
            this.tableLayoutEnvelopeFloor.Controls.Add(this.txtFloorCost, 1, 4);
            this.tableLayoutEnvelopeFloor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeFloor.Location = new System.Drawing.Point(503, 23);
            this.tableLayoutEnvelopeFloor.Name = "tableLayoutEnvelopeFloor";
            this.tableLayoutEnvelopeFloor.RowCount = 8;
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeFloor.Size = new System.Drawing.Size(484, 524);
            this.tableLayoutEnvelopeFloor.TabIndex = 1;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(3, 10);
            this.label17.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(85, 32);
            this.label17.TabIndex = 0;
            this.label17.Text = "Floor";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 121);
            this.label18.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(223, 32);
            this.label18.TabIndex = 1;
            this.label18.Text = "U-value (W/m²K)";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 179);
            this.label19.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(298, 32);
            this.label19.TabIndex = 2;
            this.label19.Text = "Emissions (kgCO2/m²)";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 237);
            this.label20.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(195, 32);
            this.label20.TabIndex = 3;
            this.label20.Text = "Cost (CHF/m²)";
            // 
            // cboFloorTemplate
            // 
            this.cboFloorTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeFloor.SetColumnSpan(this.cboFloorTemplate, 2);
            this.cboFloorTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloorTemplate.FormattingEnabled = true;
            this.cboFloorTemplate.Location = new System.Drawing.Point(3, 62);
            this.cboFloorTemplate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.cboFloorTemplate.Name = "cboFloorTemplate";
            this.cboFloorTemplate.Size = new System.Drawing.Size(478, 39);
            this.cboFloorTemplate.TabIndex = 4;
            // 
            // txtFloorUValue
            // 
            this.txtFloorUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorUValue.Location = new System.Drawing.Point(341, 121);
            this.txtFloorUValue.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtFloorUValue.Name = "txtFloorUValue";
            this.txtFloorUValue.Size = new System.Drawing.Size(140, 38);
            this.txtFloorUValue.TabIndex = 5;
            this.txtFloorUValue.Tag = "UValueFloors";
            this.txtFloorUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtFloorEmissions
            // 
            this.txtFloorEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorEmissions.Location = new System.Drawing.Point(341, 179);
            this.txtFloorEmissions.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtFloorEmissions.Name = "txtFloorEmissions";
            this.txtFloorEmissions.Size = new System.Drawing.Size(140, 38);
            this.txtFloorEmissions.TabIndex = 6;
            this.txtFloorEmissions.Tag = "EmissionsFloors";
            this.txtFloorEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtFloorCost
            // 
            this.txtFloorCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorCost.Location = new System.Drawing.Point(341, 237);
            this.txtFloorCost.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtFloorCost.Name = "txtFloorCost";
            this.txtFloorCost.Size = new System.Drawing.Size(140, 38);
            this.txtFloorCost.TabIndex = 7;
            this.txtFloorCost.Tag = "CostFloors";
            this.txtFloorCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
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
            this.tableLayoutEnvelopeWindow.Location = new System.Drawing.Point(13, 553);
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
            this.tableLayoutEnvelopeWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutEnvelopeWindow.Size = new System.Drawing.Size(484, 525);
            this.tableLayoutEnvelopeWindow.TabIndex = 2;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(3, 10);
            this.label21.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(122, 32);
            this.label21.TabIndex = 0;
            this.label21.Text = "Window";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 121);
            this.label22.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(223, 32);
            this.label22.TabIndex = 1;
            this.label22.Text = "U-value (W/m²K)";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 179);
            this.label23.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(143, 32);
            this.label23.TabIndex = 2;
            this.label23.Text = "g-value (-)";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 237);
            this.label24.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(298, 32);
            this.label24.TabIndex = 3;
            this.label24.Text = "Emissions (kgCO2/m²)";
            // 
            // cboWindowTemplate
            // 
            this.cboWindowTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeWindow.SetColumnSpan(this.cboWindowTemplate, 2);
            this.cboWindowTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWindowTemplate.FormattingEnabled = true;
            this.cboWindowTemplate.Location = new System.Drawing.Point(3, 62);
            this.cboWindowTemplate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.cboWindowTemplate.Name = "cboWindowTemplate";
            this.cboWindowTemplate.Size = new System.Drawing.Size(478, 39);
            this.cboWindowTemplate.TabIndex = 4;
            // 
            // txtWindowUValue
            // 
            this.txtWindowUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowUValue.Location = new System.Drawing.Point(341, 121);
            this.txtWindowUValue.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWindowUValue.Name = "txtWindowUValue";
            this.txtWindowUValue.Size = new System.Drawing.Size(140, 38);
            this.txtWindowUValue.TabIndex = 5;
            this.txtWindowUValue.Tag = "UValueTransparent";
            this.txtWindowUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWindowGValue
            // 
            this.txtWindowGValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowGValue.Location = new System.Drawing.Point(341, 179);
            this.txtWindowGValue.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWindowGValue.Name = "txtWindowGValue";
            this.txtWindowGValue.Size = new System.Drawing.Size(140, 38);
            this.txtWindowGValue.TabIndex = 6;
            this.txtWindowGValue.Tag = "GValue";
            this.txtWindowGValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtWindowEmissions
            // 
            this.txtWindowEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowEmissions.Location = new System.Drawing.Point(341, 237);
            this.txtWindowEmissions.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWindowEmissions.Name = "txtWindowEmissions";
            this.txtWindowEmissions.Size = new System.Drawing.Size(140, 38);
            this.txtWindowEmissions.TabIndex = 7;
            this.txtWindowEmissions.Tag = "TransparentEmissions";
            this.txtWindowEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // tableLayoutEnvelopeRoof
            // 
            this.tableLayoutEnvelopeRoof.ColumnCount = 2;
            this.tableLayoutEnvelopeRoof.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEnvelopeRoof.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label25, 0, 0);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label26, 0, 2);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label27, 0, 3);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.label28, 0, 4);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.cboRoofTemplate, 0, 1);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.txtRoofUValue, 1, 2);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.txtRoofEmissions, 1, 3);
            this.tableLayoutEnvelopeRoof.Controls.Add(this.txtRoofCost, 1, 4);
            this.tableLayoutEnvelopeRoof.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEnvelopeRoof.Location = new System.Drawing.Point(503, 553);
            this.tableLayoutEnvelopeRoof.Name = "tableLayoutEnvelopeRoof";
            this.tableLayoutEnvelopeRoof.RowCount = 8;
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutEnvelopeRoof.Size = new System.Drawing.Size(484, 525);
            this.tableLayoutEnvelopeRoof.TabIndex = 3;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(3, 10);
            this.label25.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(79, 32);
            this.label25.TabIndex = 0;
            this.label25.Text = "Roof";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(3, 121);
            this.label26.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(223, 32);
            this.label26.TabIndex = 1;
            this.label26.Text = "U-value (W/m²K)";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(3, 179);
            this.label27.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(298, 32);
            this.label27.TabIndex = 2;
            this.label27.Text = "Emissions (kgCO2/m²)";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(3, 237);
            this.label28.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(195, 32);
            this.label28.TabIndex = 3;
            this.label28.Text = "Cost (CHF/m²)";
            // 
            // cboRoofTemplate
            // 
            this.cboRoofTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnvelopeRoof.SetColumnSpan(this.cboRoofTemplate, 2);
            this.cboRoofTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoofTemplate.FormattingEnabled = true;
            this.cboRoofTemplate.Location = new System.Drawing.Point(3, 62);
            this.cboRoofTemplate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.cboRoofTemplate.Name = "cboRoofTemplate";
            this.cboRoofTemplate.Size = new System.Drawing.Size(478, 39);
            this.cboRoofTemplate.TabIndex = 4;
            // 
            // txtRoofUValue
            // 
            this.txtRoofUValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofUValue.Location = new System.Drawing.Point(341, 121);
            this.txtRoofUValue.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtRoofUValue.Name = "txtRoofUValue";
            this.txtRoofUValue.Size = new System.Drawing.Size(140, 38);
            this.txtRoofUValue.TabIndex = 5;
            this.txtRoofUValue.Tag = "UValueRoofs";
            this.txtRoofUValue.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtRoofEmissions
            // 
            this.txtRoofEmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofEmissions.Location = new System.Drawing.Point(341, 179);
            this.txtRoofEmissions.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtRoofEmissions.Name = "txtRoofEmissions";
            this.txtRoofEmissions.Size = new System.Drawing.Size(140, 38);
            this.txtRoofEmissions.TabIndex = 6;
            this.txtRoofEmissions.Tag = "EmissionsRoofs";
            this.txtRoofEmissions.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtRoofCost
            // 
            this.txtRoofCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRoofCost.Location = new System.Drawing.Point(341, 237);
            this.txtRoofCost.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtRoofCost.Name = "txtRoofCost";
            this.txtRoofCost.Size = new System.Drawing.Size(140, 38);
            this.txtRoofCost.TabIndex = 7;
            this.txtRoofCost.Tag = "CostRoofs";
            this.txtRoofCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 295);
            this.label29.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(195, 32);
            this.label29.TabIndex = 8;
            this.label29.Text = "Cost (CHF/m²)";
            // 
            // txtWindowCost
            // 
            this.txtWindowCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowCost.Location = new System.Drawing.Point(341, 295);
            this.txtWindowCost.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWindowCost.Name = "txtWindowCost";
            this.txtWindowCost.Size = new System.Drawing.Size(140, 38);
            this.txtWindowCost.TabIndex = 9;
            this.txtWindowCost.Tag = "TransparentCost";
            this.txtWindowCost.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(3, 353);
            this.label30.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(205, 32);
            this.label30.TabIndex = 10;
            this.label30.Text = "g-value total (-)";
            // 
            // txtWindowGValueTotal
            // 
            this.txtWindowGValueTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowGValueTotal.Location = new System.Drawing.Point(341, 353);
            this.txtWindowGValueTotal.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWindowGValueTotal.Name = "txtWindowGValueTotal";
            this.txtWindowGValueTotal.Size = new System.Drawing.Size(140, 38);
            this.txtWindowGValueTotal.TabIndex = 11;
            this.txtWindowGValueTotal.Tag = "GValueTotal";
            this.txtWindowGValueTotal.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(3, 411);
            this.label31.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(320, 32);
            this.label31.TabIndex = 12;
            this.label31.Text = "Shading setpoint (W/m²)";
            // 
            // txtWindowShadingSetpoint
            // 
            this.txtWindowShadingSetpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowShadingSetpoint.Location = new System.Drawing.Point(341, 411);
            this.txtWindowShadingSetpoint.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtWindowShadingSetpoint.Name = "txtWindowShadingSetpoint";
            this.txtWindowShadingSetpoint.Size = new System.Drawing.Size(140, 38);
            this.txtWindowShadingSetpoint.TabIndex = 13;
            this.txtWindowShadingSetpoint.Tag = "ShadingSetpoint";
            this.txtWindowShadingSetpoint.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
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
            this.tableLayoutInternalGains.Name = "tableLayoutInternalGains";
            this.tableLayoutInternalGains.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            this.tableLayoutInternalGains.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutInternalGains.RowCount = 7;
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInternalGains.Size = new System.Drawing.Size(1006, 1097);
            this.tableLayoutInternalGains.TabIndex = 0;
            // 
            // label33
            // 
            this.label33.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(13, 30);
            this.label33.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(487, 32);
            this.label33.TabIndex = 1;
            this.label33.Text = "People heat (W/m²)";
            // 
            // label32
            // 
            this.label32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(506, 30);
            this.label32.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(487, 32);
            this.label32.TabIndex = 2;
            this.label32.Text = "People load hours (h)";
            // 
            // label34
            // 
            this.label34.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(13, 140);
            this.label34.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(487, 32);
            this.label34.TabIndex = 3;
            this.label34.Text = "Light heat (W/m²)";
            // 
            // label35
            // 
            this.label35.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(506, 140);
            this.label35.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(487, 32);
            this.label35.TabIndex = 4;
            this.label35.Text = "Light load hours (h)";
            // 
            // label36
            // 
            this.label36.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(13, 250);
            this.label36.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(487, 32);
            this.label36.TabIndex = 5;
            this.label36.Text = "Equipment heat (W/m²)";
            // 
            // label37
            // 
            this.label37.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(506, 250);
            this.label37.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(487, 32);
            this.label37.TabIndex = 6;
            this.label37.Text = "Equipment load hours";
            // 
            // txtOccupantLoads
            // 
            this.txtOccupantLoads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOccupantLoads.Location = new System.Drawing.Point(50, 82);
            this.txtOccupantLoads.Margin = new System.Windows.Forms.Padding(40, 10, 40, 10);
            this.txtOccupantLoads.Name = "txtOccupantLoads";
            this.txtOccupantLoads.Size = new System.Drawing.Size(413, 38);
            this.txtOccupantLoads.TabIndex = 7;
            this.txtOccupantLoads.Tag = "OccupantLoads";
            this.txtOccupantLoads.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtOccupantYearlyHours
            // 
            this.txtOccupantYearlyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOccupantYearlyHours.Location = new System.Drawing.Point(543, 82);
            this.txtOccupantYearlyHours.Margin = new System.Windows.Forms.Padding(40, 10, 40, 10);
            this.txtOccupantYearlyHours.Name = "txtOccupantYearlyHours";
            this.txtOccupantYearlyHours.Size = new System.Drawing.Size(413, 38);
            this.txtOccupantYearlyHours.TabIndex = 8;
            this.txtOccupantYearlyHours.Tag = "OccupantYearlyHours";
            this.txtOccupantYearlyHours.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtLightingLoads
            // 
            this.txtLightingLoads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLightingLoads.Location = new System.Drawing.Point(50, 192);
            this.txtLightingLoads.Margin = new System.Windows.Forms.Padding(40, 10, 40, 10);
            this.txtLightingLoads.Name = "txtLightingLoads";
            this.txtLightingLoads.Size = new System.Drawing.Size(413, 38);
            this.txtLightingLoads.TabIndex = 9;
            this.txtLightingLoads.Tag = "LightingLoads";
            this.txtLightingLoads.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtLightingYearlyHours
            // 
            this.txtLightingYearlyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLightingYearlyHours.Location = new System.Drawing.Point(543, 192);
            this.txtLightingYearlyHours.Margin = new System.Windows.Forms.Padding(40, 10, 40, 10);
            this.txtLightingYearlyHours.Name = "txtLightingYearlyHours";
            this.txtLightingYearlyHours.Size = new System.Drawing.Size(413, 38);
            this.txtLightingYearlyHours.TabIndex = 10;
            this.txtLightingYearlyHours.Tag = "LightingYearlyHours";
            this.txtLightingYearlyHours.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtEquipmentLoads
            // 
            this.txtEquipmentLoads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEquipmentLoads.Location = new System.Drawing.Point(50, 302);
            this.txtEquipmentLoads.Margin = new System.Windows.Forms.Padding(40, 10, 40, 10);
            this.txtEquipmentLoads.Name = "txtEquipmentLoads";
            this.txtEquipmentLoads.Size = new System.Drawing.Size(413, 38);
            this.txtEquipmentLoads.TabIndex = 11;
            this.txtEquipmentLoads.Tag = "EquipmentLoads";
            this.txtEquipmentLoads.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // txtEquipmentYearlyHours
            // 
            this.txtEquipmentYearlyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEquipmentYearlyHours.Location = new System.Drawing.Point(543, 302);
            this.txtEquipmentYearlyHours.Margin = new System.Windows.Forms.Padding(40, 10, 40, 10);
            this.txtEquipmentYearlyHours.Name = "txtEquipmentYearlyHours";
            this.txtEquipmentYearlyHours.Size = new System.Drawing.Size(413, 38);
            this.txtEquipmentYearlyHours.TabIndex = 12;
            this.txtEquipmentYearlyHours.Tag = "EquipmentYearlyHours";
            this.txtEquipmentYearlyHours.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_TextChanged);
            // 
            // BuildingInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 1155);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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
            this.tabIG.ResumeLayout(false);
            this.tableLayoutEnvelopeMain.ResumeLayout(false);
            this.tableLayoutEnvelopeWall.ResumeLayout(false);
            this.tableLayoutEnvelopeWall.PerformLayout();
            this.tableLayoutEnvelopeFloor.ResumeLayout(false);
            this.tableLayoutEnvelopeFloor.PerformLayout();
            this.tableLayoutEnvelopeWindow.ResumeLayout(false);
            this.tableLayoutEnvelopeWindow.PerformLayout();
            this.tableLayoutEnvelopeRoof.ResumeLayout(false);
            this.tableLayoutEnvelopeRoof.PerformLayout();
            this.tableLayoutInternalGains.ResumeLayout(false);
            this.tableLayoutInternalGains.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSIA;
        private System.Windows.Forms.TabPage tabENV;
        private System.Windows.Forms.TabPage tabIG;
        private System.Windows.Forms.TabPage tabVEN;
        private System.Windows.Forms.TableLayoutPanel tableLayoutSiaMain;
        private System.Windows.Forms.Panel panelSiaProperties;
        private System.Windows.Forms.TableLayoutPanel tableLayoutSiaProperties;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboBuildingUseType;
        private System.Windows.Forms.ComboBox cboRoomType;
        private System.Windows.Forms.ComboBox cboBuildingQuality;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtFloorArea;
        private System.Windows.Forms.TextBox txtWallArea;
        private System.Windows.Forms.TextBox txtWinArea;
        private System.Windows.Forms.TextBox txtRoofArea;
        private System.Windows.Forms.TextBox txtHeatingSetPoint;
        private System.Windows.Forms.TextBox txtCoolingSetPoint;
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
    }
}