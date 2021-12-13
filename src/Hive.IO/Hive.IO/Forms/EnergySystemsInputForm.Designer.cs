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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.gridConversion = new System.Windows.Forms.DataGridView();
            this.ConversionProperties = new Hive.IO.Forms.Controls.ConversionTechPropertiesBase();
            this.tabEmission = new System.Windows.Forms.TabPage();
            this.tableLayoutEmission = new System.Windows.Forms.TableLayoutPanel();
            this.gridEmission = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.emitterProperties = new Hive.IO.Forms.Controls.EmitterProperties();
            this.AddDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Conversion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndUse = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.tabControl.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1683, 831);
            this.tabControl.TabIndex = 0;
            // 
            // tabConversion
            // 
            this.tabConversion.Controls.Add(this.tableLayoutPanelMain);
            this.tabConversion.Location = new System.Drawing.Point(8, 39);
            this.tabConversion.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabConversion.Name = "tabConversion";
            this.tabConversion.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabConversion.Size = new System.Drawing.Size(1667, 784);
            this.tabConversion.TabIndex = 0;
            this.tabConversion.Text = "Conversion";
            this.tabConversion.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.gridConversion, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.ConversionProperties, 0, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1655, 772);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // gridConversion
            // 
            this.gridConversion.AllowUserToResizeColumns = false;
            this.gridConversion.AllowUserToResizeRows = false;
            this.gridConversion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConversion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AddDelete,
            this.Conversion,
            this.Source,
            this.EndUse});
            this.gridConversion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridConversion.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridConversion.Location = new System.Drawing.Point(6, 6);
            this.gridConversion.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gridConversion.MultiSelect = false;
            this.gridConversion.Name = "gridConversion";
            this.gridConversion.RowHeadersVisible = false;
            this.gridConversion.RowHeadersWidth = 102;
            this.gridConversion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridConversion.Size = new System.Drawing.Size(1643, 219);
            this.gridConversion.TabIndex = 0;
            this.gridConversion.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridConversion_CellContentClick);
            this.gridConversion.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridConversion_CellValueChanged);
            this.gridConversion.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridConversion_CurrentCellDirtyStateChanged);
            this.gridConversion.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridConversion_DataBindingComplete);
            this.gridConversion.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.gridConversion.SelectionChanged += new System.EventHandler(this.gridConversion_SelectionChanged);
            // 
            // ConversionProperties
            // 
            this.ConversionProperties.BackColor = System.Drawing.SystemColors.Window;
            this.ConversionProperties.Conversion = null;
            this.ConversionProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConversionProperties.Location = new System.Drawing.Point(2, 233);
            this.ConversionProperties.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ConversionProperties.Name = "ConversionProperties";
            this.ConversionProperties.Size = new System.Drawing.Size(1651, 537);
            this.ConversionProperties.TabIndex = 1;
            // 
            // tabEmission
            // 
            this.tabEmission.Controls.Add(this.tableLayoutEmission);
            this.tabEmission.Location = new System.Drawing.Point(8, 39);
            this.tabEmission.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabEmission.Name = "tabEmission";
            this.tabEmission.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabEmission.Size = new System.Drawing.Size(1667, 784);
            this.tabEmission.TabIndex = 1;
            this.tabEmission.Text = "Emission";
            this.tabEmission.UseVisualStyleBackColor = true;
            // 
            // tableLayoutEmission
            // 
            this.tableLayoutEmission.ColumnCount = 1;
            this.tableLayoutEmission.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutEmission.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutEmission.Controls.Add(this.gridEmission, 0, 0);
            this.tableLayoutEmission.Controls.Add(this.emitterProperties, 0, 1);
            this.tableLayoutEmission.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEmission.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutEmission.Name = "tableLayoutEmission";
            this.tableLayoutEmission.RowCount = 2;
            this.tableLayoutEmission.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutEmission.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutEmission.Size = new System.Drawing.Size(1655, 772);
            this.tableLayoutEmission.TabIndex = 0;
            // 
            // gridEmission
            // 
            this.gridEmission.AllowUserToResizeColumns = false;
            this.gridEmission.AllowUserToResizeRows = false;
            this.gridEmission.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEmission.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewComboBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.gridEmission.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEmission.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridEmission.Location = new System.Drawing.Point(6, 6);
            this.gridEmission.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gridEmission.MultiSelect = false;
            this.gridEmission.Name = "gridEmission";
            this.gridEmission.RowHeadersVisible = false;
            this.gridEmission.RowHeadersWidth = 102;
            this.gridEmission.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridEmission.Size = new System.Drawing.Size(1643, 219);
            this.gridEmission.TabIndex = 1;
            this.gridEmission.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridEmission_CellValueChanged);
            this.gridEmission.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridEmission_CurrentCellDirtyStateChanged);
            this.gridEmission.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridEmission_DataBindingComplete);
            this.gridEmission.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.gridEmission.SelectionChanged += new System.EventHandler(this.gridEmission_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Source";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 12;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 250;
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.HeaderText = "Conversion";
            this.dataGridViewComboBoxColumn1.MinimumWidth = 12;
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.Width = 250;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "EndUse";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 12;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 250;
            // 
            // emitterProperties
            // 
            this.emitterProperties.BackColor = System.Drawing.SystemColors.Window;
            this.emitterProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emitterProperties.Emitter = null;
            this.emitterProperties.Location = new System.Drawing.Point(2, 233);
            this.emitterProperties.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.emitterProperties.Name = "emitterProperties";
            this.emitterProperties.Size = new System.Drawing.Size(1651, 537);
            this.emitterProperties.TabIndex = 2;
            // 
            // AddDelete
            // 
            this.AddDelete.HeaderText = "Edit";
            this.AddDelete.MinimumWidth = 10;
            this.AddDelete.Name = "AddDelete";
            this.AddDelete.Width = 200;
            // 
            // Conversion
            // 
            this.Conversion.HeaderText = "Conversion";
            this.Conversion.MinimumWidth = 12;
            this.Conversion.Name = "Conversion";
            this.Conversion.Width = 250;
            // 
            // Source
            // 
            this.Source.HeaderText = "Source";
            this.Source.MinimumWidth = 12;
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            this.Source.Width = 250;
            // 
            // EndUse
            // 
            this.EndUse.HeaderText = "EndUse";
            this.EndUse.MinimumWidth = 12;
            this.EndUse.Name = "EndUse";
            this.EndUse.ReadOnly = true;
            this.EndUse.Width = 250;
            // 
            // EnergySystemsInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1683, 831);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Controls.EmitterProperties emitterProperties;
        private System.Windows.Forms.DataGridViewButtonColumn AddDelete;
        private System.Windows.Forms.DataGridViewComboBoxColumn Conversion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndUse;
    }
}