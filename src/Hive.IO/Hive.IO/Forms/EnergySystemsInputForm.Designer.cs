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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gridConversion = new System.Windows.Forms.DataGridView();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conversion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EndUse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabEmission = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabConversion.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridConversion)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabConversion);
            this.tabControl.Controls.Add(this.tabEmission);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1214, 704);
            this.tabControl.TabIndex = 0;
            // 
            // tabConversion
            // 
            this.tabConversion.Controls.Add(this.tableLayoutPanel1);
            this.tabConversion.Location = new System.Drawing.Point(4, 22);
            this.tabConversion.Name = "tabConversion";
            this.tabConversion.Padding = new System.Windows.Forms.Padding(3);
            this.tabConversion.Size = new System.Drawing.Size(1206, 678);
            this.tabConversion.TabIndex = 0;
            this.tabConversion.Text = "Conversion";
            this.tabConversion.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gridConversion, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1200, 672);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gridConversion
            // 
            this.gridConversion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConversion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Source,
            this.Conversion,
            this.EndUse});
            this.gridConversion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridConversion.Location = new System.Drawing.Point(3, 3);
            this.gridConversion.MultiSelect = false;
            this.gridConversion.Name = "gridConversion";
            this.gridConversion.Size = new System.Drawing.Size(1194, 262);
            this.gridConversion.TabIndex = 0;
            // 
            // Source
            // 
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            // 
            // Conversion
            // 
            this.Conversion.HeaderText = "Conversion";
            this.Conversion.Name = "Conversion";
            // 
            // EndUse
            // 
            this.EndUse.HeaderText = "EndUse";
            this.EndUse.Name = "EndUse";
            this.EndUse.ReadOnly = true;
            // 
            // tabEmission
            // 
            this.tabEmission.Location = new System.Drawing.Point(4, 22);
            this.tabEmission.Name = "tabEmission";
            this.tabEmission.Padding = new System.Windows.Forms.Padding(3);
            this.tabEmission.Size = new System.Drawing.Size(1206, 678);
            this.tabEmission.TabIndex = 1;
            this.tabEmission.Text = "Emission";
            this.tabEmission.UseVisualStyleBackColor = true;
            // 
            // EnergySystemsInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1214, 704);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EnergySystemsInputForm";
            this.Text = "EnergySystemsInputForm";
            this.Load += new System.EventHandler(this.EnergySystemsInputForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabConversion.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridConversion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabConversion;
        private System.Windows.Forms.TabPage tabEmission;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView gridConversion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewComboBoxColumn Conversion;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndUse;
    }
}