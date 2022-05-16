namespace Hive.IO.Forms
{
    partial class EnergySystemsInputForm
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
            this.tabConversion = new System.Windows.Forms.TabPage();
            this.deleteConversionTech = new System.Windows.Forms.Button();
            this.addconversionTech = new System.Windows.Forms.Button();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.gridConversion = new System.Windows.Forms.DataGridView();
            this.Conversion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndUse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabEmission = new System.Windows.Forms.TabPage();
            this.deleteEmission = new System.Windows.Forms.Button();
            this.addEmission = new System.Windows.Forms.Button();
            this.tableLayoutEmission = new System.Windows.Forms.TableLayoutPanel();
            this.gridEmission = new System.Windows.Forms.DataGridView();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConversionProperties = new Hive.IO.Forms.Controls.ConversionTechPropertiesBase();
            this.emitterProperties = new Hive.IO.Forms.Controls.EmitterProperties();
            this.tabControl.SuspendLayout();
            this.tabConversion.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridConversion)).BeginInit();
            this.tabEmission.SuspendLayout();
            this.tableLayoutEmission.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEmission)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabConversion);
            this.tabControl.Controls.Add(this.tabEmission);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(6);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1679, 825);
            this.tabControl.TabIndex = 0;
            // 
            // tabConversion
            // 
            this.tabConversion.Controls.Add(this.deleteConversionTech);
            this.tabConversion.Controls.Add(this.addconversionTech);
            this.tabConversion.Controls.Add(this.tableLayoutPanelMain);
            this.tabConversion.Location = new System.Drawing.Point(8, 39);
            this.tabConversion.Margin = new System.Windows.Forms.Padding(6);
            this.tabConversion.Name = "tabConversion";
            this.tabConversion.Padding = new System.Windows.Forms.Padding(6);
            this.tabConversion.Size = new System.Drawing.Size(1663, 778);
            this.tabConversion.TabIndex = 0;
            this.tabConversion.Text = "Conversion";
            this.tabConversion.UseVisualStyleBackColor = true;
            // 
            // deleteConversionTech
            // 
            this.deleteConversionTech.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteConversionTech.Location = new System.Drawing.Point(47, 0);
            this.deleteConversionTech.Name = "deleteConversionTech";
            this.deleteConversionTech.Size = new System.Drawing.Size(45, 45);
            this.deleteConversionTech.TabIndex = 3;
            this.deleteConversionTech.Text = "-";
            this.deleteConversionTech.UseVisualStyleBackColor = true;
            this.deleteConversionTech.Click += new System.EventHandler(this.deleteConversionTech_Click);
            // 
            // addconversionTech
            // 
            this.addconversionTech.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addconversionTech.Location = new System.Drawing.Point(0, 0);
            this.addconversionTech.Name = "addconversionTech";
            this.addconversionTech.Size = new System.Drawing.Size(45, 45);
            this.addconversionTech.TabIndex = 2;
            this.addconversionTech.Text = "+";
            this.addconversionTech.UseVisualStyleBackColor = true;
            this.addconversionTech.Click += new System.EventHandler(this.addConversionTech_Click);
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.gridConversion, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.ConversionProperties, 0, 1);
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(-8, 42);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.42776F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.57224F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1671, 738);
            this.tableLayoutPanelMain.TabIndex = 0;
            this.tableLayoutPanelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanelMain_Paint);
            // 
            // gridConversion
            // 
            this.gridConversion.AllowUserToAddRows = false;
            this.gridConversion.AllowUserToDeleteRows = false;
            this.gridConversion.AllowUserToResizeColumns = false;
            this.gridConversion.AllowUserToResizeRows = false;
            this.gridConversion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConversion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Conversion,
            this.Source,
            this.EndUse});
            this.gridConversion.Dock = System.Windows.Forms.DockStyle.Right;
            this.gridConversion.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridConversion.Location = new System.Drawing.Point(8, 6);
            this.gridConversion.Margin = new System.Windows.Forms.Padding(6);
            this.gridConversion.MultiSelect = false;
            this.gridConversion.Name = "gridConversion";
            this.gridConversion.RowHeadersVisible = false;
            this.gridConversion.RowHeadersWidth = 102;
            this.gridConversion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridConversion.Size = new System.Drawing.Size(1657, 234);
            this.gridConversion.TabIndex = 0;
            this.gridConversion.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridConversion_CellValueChanged);
            this.gridConversion.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridConversion_CurrentCellDirtyStateChanged);
            this.gridConversion.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridConversion_DataBindingComplete);
            this.gridConversion.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.gridConversion.SelectionChanged += new System.EventHandler(this.gridConversion_SelectionChanged);
            // 
            // Conversion
            // 
            this.Conversion.HeaderText = "Conversion";
            this.Conversion.MinimumWidth = 12;
            this.Conversion.Name = "Conversion";
            this.Conversion.Width = 127;
            // 
            // Source
            // 
            this.Source.HeaderText = "Source";
            this.Source.MinimumWidth = 12;
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            this.Source.Width = 125;
            // 
            // EndUse
            // 
            this.EndUse.HeaderText = "EndUse";
            this.EndUse.MinimumWidth = 12;
            this.EndUse.Name = "EndUse";
            this.EndUse.ReadOnly = true;
            this.EndUse.Width = 133;
            // 
            // tabEmission
            // 
            this.tabEmission.Controls.Add(this.deleteEmission);
            this.tabEmission.Controls.Add(this.addEmission);
            this.tabEmission.Controls.Add(this.tableLayoutEmission);
            this.tabEmission.Location = new System.Drawing.Point(8, 39);
            this.tabEmission.Margin = new System.Windows.Forms.Padding(6);
            this.tabEmission.Name = "tabEmission";
            this.tabEmission.Padding = new System.Windows.Forms.Padding(6);
            this.tabEmission.Size = new System.Drawing.Size(1663, 778);
            this.tabEmission.TabIndex = 1;
            this.tabEmission.Text = "Emission";
            this.tabEmission.UseVisualStyleBackColor = true;
            // 
            // deleteEmission
            // 
            this.deleteEmission.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteEmission.Location = new System.Drawing.Point(47, 0);
            this.deleteEmission.Name = "deleteEmission";
            this.deleteEmission.Size = new System.Drawing.Size(45, 45);
            this.deleteEmission.TabIndex = 2;
            this.deleteEmission.Text = "-";
            this.deleteEmission.UseVisualStyleBackColor = true;
            this.deleteEmission.Click += new System.EventHandler(this.deleteEmission_Click);
            // 
            // addEmission
            // 
            this.addEmission.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addEmission.Location = new System.Drawing.Point(0, 0);
            this.addEmission.Name = "addEmission";
            this.addEmission.Size = new System.Drawing.Size(45, 45);
            this.addEmission.TabIndex = 1;
            this.addEmission.Text = "+";
            this.addEmission.UseVisualStyleBackColor = true;
            this.addEmission.Click += new System.EventHandler(this.addEmission_Click);
            // 
            // tableLayoutEmission
            // 
            this.tableLayoutEmission.ColumnCount = 1;
            this.tableLayoutEmission.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutEmission.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutEmission.Controls.Add(this.gridEmission, 0, 0);
            this.tableLayoutEmission.Controls.Add(this.emitterProperties, 0, 1);
            this.tableLayoutEmission.Location = new System.Drawing.Point(-8, 41);
            this.tableLayoutEmission.Name = "tableLayoutEmission";
            this.tableLayoutEmission.RowCount = 2;
            this.tableLayoutEmission.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.25389F));
            this.tableLayoutEmission.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 67.74612F));
            this.tableLayoutEmission.Size = new System.Drawing.Size(1669, 737);
            this.tableLayoutEmission.TabIndex = 0;
            // 
            // gridEmission
            // 
            this.gridEmission.AllowUserToAddRows = false;
            this.gridEmission.AllowUserToDeleteRows = false;
            this.gridEmission.AllowUserToResizeColumns = false;
            this.gridEmission.AllowUserToResizeRows = false;
            this.gridEmission.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEmission.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewComboBoxColumn1,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.gridEmission.Dock = System.Windows.Forms.DockStyle.Right;
            this.gridEmission.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridEmission.Location = new System.Drawing.Point(8, 6);
            this.gridEmission.Margin = new System.Windows.Forms.Padding(6);
            this.gridEmission.MultiSelect = false;
            this.gridEmission.Name = "gridEmission";
            this.gridEmission.RowHeadersVisible = false;
            this.gridEmission.RowHeadersWidth = 102;
            this.gridEmission.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridEmission.Size = new System.Drawing.Size(1655, 225);
            this.gridEmission.TabIndex = 1;
            this.gridEmission.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridEmission_CellValueChanged);
            this.gridEmission.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridEmission_CurrentCellDirtyStateChanged);
            this.gridEmission.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridEmission_DataBindingComplete);
            this.gridEmission.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.gridEmission.SelectionChanged += new System.EventHandler(this.gridEmission_SelectionChanged);
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.HeaderText = "Conversion";
            this.dataGridViewComboBoxColumn1.MinimumWidth = 12;
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.Width = 250;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Source";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 12;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 250;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "EndUse";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 12;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 250;
            // 
            // ConversionProperties
            // 
            this.ConversionProperties.BackColor = System.Drawing.SystemColors.Window;
            this.ConversionProperties.Conversion = null;
            this.ConversionProperties.Location = new System.Drawing.Point(2, 248);
            this.ConversionProperties.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ConversionProperties.Name = "ConversionProperties";
            this.ConversionProperties.Size = new System.Drawing.Size(1661, 485);
            this.ConversionProperties.TabIndex = 1;
            // 
            // emitterProperties
            // 
            this.emitterProperties.BackColor = System.Drawing.SystemColors.Window;
            this.emitterProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.emitterProperties.Emitter = null;
            this.emitterProperties.Location = new System.Drawing.Point(14, 239);
            this.emitterProperties.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.emitterProperties.Name = "emitterProperties";
            this.emitterProperties.Size = new System.Drawing.Size(1653, 496);
            this.emitterProperties.TabIndex = 2;
            // 
            // EnergySystemsInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1679, 825);
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "EnergySystemsInputForm";
            this.Text = "EnergySystemsInputForm";
            this.Load += new System.EventHandler(this.EnergySystemsInputForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabConversion.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridConversion)).EndInit();
            this.tabEmission.ResumeLayout(false);
            this.tableLayoutEmission.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridEmission)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabConversion;
        private System.Windows.Forms.TabPage tabEmission;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.DataGridView gridConversion;
        private Controls.ConversionTechPropertiesBase ConversionProperties;
        private System.Windows.Forms.TableLayoutPanel tableLayoutEmission;
        private System.Windows.Forms.DataGridView gridEmission;
        private Controls.EmitterProperties emitterProperties;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn Conversion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndUse;
        private System.Windows.Forms.Button addconversionTech;
        private System.Windows.Forms.Button deleteConversionTech;
        private System.Windows.Forms.Button deleteEmission;
        private System.Windows.Forms.Button addEmission;
    }
}