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
            this.tabENV = new System.Windows.Forms.TabPage();
            this.tabIG = new System.Windows.Forms.TabPage();
            this.tabVEN = new System.Windows.Forms.TabPage();
            this.tableLayoutSiaMain = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutSiaProperties = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBuildingUseType = new System.Windows.Forms.ComboBox();
            this.cboRoomType = new System.Windows.Forms.ComboBox();
            this.cboBuildingQuality = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
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
            this.tabControl.SuspendLayout();
            this.tabSIA.SuspendLayout();
            this.tableLayoutSiaMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutSiaProperties.SuspendLayout();
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
            // tabENV
            // 
            this.tabENV.Location = new System.Drawing.Point(10, 48);
            this.tabENV.Name = "tabENV";
            this.tabENV.Padding = new System.Windows.Forms.Padding(3);
            this.tabENV.Size = new System.Drawing.Size(780, 966);
            this.tabENV.TabIndex = 1;
            this.tabENV.Text = "Envelope";
            this.tabENV.UseVisualStyleBackColor = true;
            // 
            // tabIG
            // 
            this.tabIG.Location = new System.Drawing.Point(10, 48);
            this.tabIG.Name = "tabIG";
            this.tabIG.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabIG.Size = new System.Drawing.Size(780, 966);
            this.tabIG.TabIndex = 2;
            this.tabIG.Text = "Internal Gains";
            this.tabIG.UseVisualStyleBackColor = true;
            // 
            // tabVEN
            // 
            this.tabVEN.Location = new System.Drawing.Point(10, 48);
            this.tabVEN.Name = "tabVEN";
            this.tabVEN.Size = new System.Drawing.Size(780, 966);
            this.tabVEN.TabIndex = 3;
            this.tabVEN.Text = "Ventilation";
            this.tabVEN.UseVisualStyleBackColor = true;
            // 
            // tableLayoutSiaMain
            // 
            this.tableLayoutSiaMain.ColumnCount = 2;
            this.tableLayoutSiaMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSiaMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSiaMain.Controls.Add(this.panel1, 0, 4);
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
            this.tableLayoutSiaMain.RowCount = 5;
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutSiaMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaMain.Size = new System.Drawing.Size(1000, 1091);
            this.tableLayoutSiaMain.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.tableLayoutSiaMain.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.tableLayoutSiaProperties);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 880);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(994, 208);
            this.panel1.TabIndex = 0;
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
            this.tableLayoutSiaProperties.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutSiaProperties.Name = "tableLayoutSiaProperties";
            this.tableLayoutSiaProperties.RowCount = 5;
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutSiaProperties.Size = new System.Drawing.Size(994, 208);
            this.tableLayoutSiaProperties.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Building use type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "Room type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 32);
            this.label3.TabIndex = 3;
            this.label3.Text = "Building quality";
            // 
            // cboBuildingUseType
            // 
            this.cboBuildingUseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingUseType.FormattingEnabled = true;
            this.cboBuildingUseType.Location = new System.Drawing.Point(503, 3);
            this.cboBuildingUseType.Name = "cboBuildingUseType";
            this.cboBuildingUseType.Size = new System.Drawing.Size(494, 39);
            this.cboBuildingUseType.TabIndex = 4;
            // 
            // cboRoomType
            // 
            this.cboRoomType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRoomType.FormattingEnabled = true;
            this.cboRoomType.Location = new System.Drawing.Point(503, 49);
            this.cboRoomType.Name = "cboRoomType";
            this.cboRoomType.Size = new System.Drawing.Size(494, 39);
            this.cboRoomType.TabIndex = 5;
            // 
            // cboBuildingQuality
            // 
            this.cboBuildingQuality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBuildingQuality.FormattingEnabled = true;
            this.cboBuildingQuality.Location = new System.Drawing.Point(503, 95);
            this.cboBuildingQuality.Name = "cboBuildingQuality";
            this.cboBuildingQuality.Size = new System.Drawing.Size(494, 39);
            this.cboBuildingQuality.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.tableLayoutSiaMain.SetColumnSpan(this.label4, 2);
            this.label4.Location = new System.Drawing.Point(3, 803);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.label4.Size = new System.Drawing.Size(994, 74);
            this.label4.TabIndex = 7;
            this.label4.Text = "Changing building quality and use types will impact the envelope, internal gains " +
    "and ventilation parameters";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 32);
            this.label5.TabIndex = 0;
            this.label5.Text = "Geometry";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(499, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 32);
            this.label6.TabIndex = 1;
            this.label6.Text = "Set-points";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(204, 32);
            this.label7.TabIndex = 2;
            this.label7.Text = "Floor Area (m²)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(195, 32);
            this.label8.TabIndex = 3;
            this.label8.Text = "Wall Area (m²)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(499, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 32);
            this.label9.TabIndex = 4;
            this.label9.Text = "Heating";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(499, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 32);
            this.label10.TabIndex = 5;
            this.label10.Text = "Cooling";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(188, 32);
            this.label11.TabIndex = 6;
            this.label11.Text = "Win Area (m²)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 164);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(199, 32);
            this.label12.TabIndex = 7;
            this.label12.Text = "Roof Area (m²)";
            // 
            // txtFloorArea
            // 
            this.txtFloorArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloorArea.Location = new System.Drawing.Point(251, 35);
            this.txtFloorArea.Name = "txtFloorArea";
            this.txtFloorArea.Size = new System.Drawing.Size(242, 38);
            this.txtFloorArea.TabIndex = 8;
            // 
            // txtWallArea
            // 
            this.txtWallArea.Location = new System.Drawing.Point(251, 79);
            this.txtWallArea.Name = "txtWallArea";
            this.txtWallArea.Size = new System.Drawing.Size(100, 38);
            this.txtWallArea.TabIndex = 9;
            // 
            // txtWinArea
            // 
            this.txtWinArea.Location = new System.Drawing.Point(251, 123);
            this.txtWinArea.Name = "txtWinArea";
            this.txtWinArea.Size = new System.Drawing.Size(100, 38);
            this.txtWinArea.TabIndex = 10;
            // 
            // txtRoofArea
            // 
            this.txtRoofArea.Location = new System.Drawing.Point(251, 167);
            this.txtRoofArea.Name = "txtRoofArea";
            this.txtRoofArea.Size = new System.Drawing.Size(100, 38);
            this.txtRoofArea.TabIndex = 11;
            // 
            // txtHeatingSetPoint
            // 
            this.txtHeatingSetPoint.Location = new System.Drawing.Point(747, 35);
            this.txtHeatingSetPoint.Name = "txtHeatingSetPoint";
            this.txtHeatingSetPoint.Size = new System.Drawing.Size(100, 38);
            this.txtHeatingSetPoint.TabIndex = 12;
            // 
            // txtCoolingSetPoint
            // 
            this.txtCoolingSetPoint.Location = new System.Drawing.Point(747, 79);
            this.txtCoolingSetPoint.Name = "txtCoolingSetPoint";
            this.txtCoolingSetPoint.Size = new System.Drawing.Size(100, 38);
            this.txtCoolingSetPoint.TabIndex = 13;
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
            this.tabControl.ResumeLayout(false);
            this.tabSIA.ResumeLayout(false);
            this.tableLayoutSiaMain.ResumeLayout(false);
            this.tableLayoutSiaMain.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutSiaProperties.ResumeLayout(false);
            this.tableLayoutSiaProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSIA;
        private System.Windows.Forms.TabPage tabENV;
        private System.Windows.Forms.TabPage tabIG;
        private System.Windows.Forms.TabPage tabVEN;
        private System.Windows.Forms.TableLayoutPanel tableLayoutSiaMain;
        private System.Windows.Forms.Panel panel1;
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
    }
}