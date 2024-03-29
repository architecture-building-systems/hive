﻿namespace Hive.IO.Forms
{
    partial class VisualizerPlotProperties
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnResetDemandAxis = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtEnergyDemandMinimum = new System.Windows.Forms.TextBox();
            this.txtEnergyDemandMaximum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnResetSolarGainsAxis = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtSolarGainsMinimum = new System.Windows.Forms.TextBox();
            this.txtSolarGainsMaximum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(656, 361);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnResetDemandAxis);
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(648, 323);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Energy demand (Total Monthly)";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnResetDemandAxis
            // 
            this.btnResetDemandAxis.Location = new System.Drawing.Point(2, 98);
            this.btnResetDemandAxis.Name = "btnResetDemandAxis";
            this.btnResetDemandAxis.Size = new System.Drawing.Size(158, 51);
            this.btnResetDemandAxis.TabIndex = 1;
            this.btnResetDemandAxis.Text = "Reset Axis";
            this.btnResetDemandAxis.UseVisualStyleBackColor = true;
            this.btnResetDemandAxis.Click += new System.EventHandler(this.btnResetDemandAxis_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.txtEnergyDemandMinimum, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtEnergyDemandMaximum, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 81);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtEnergyDemandMinimum
            // 
            this.txtEnergyDemandMinimum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEnergyDemandMinimum.Location = new System.Drawing.Point(324, 2);
            this.txtEnergyDemandMinimum.Margin = new System.Windows.Forms.Padding(2);
            this.txtEnergyDemandMinimum.Name = "txtEnergyDemandMinimum";
            this.txtEnergyDemandMinimum.Size = new System.Drawing.Size(318, 31);
            this.txtEnergyDemandMinimum.TabIndex = 0;
            // 
            // txtEnergyDemandMaximum
            // 
            this.txtEnergyDemandMaximum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEnergyDemandMaximum.Location = new System.Drawing.Point(324, 42);
            this.txtEnergyDemandMaximum.Margin = new System.Windows.Forms.Padding(2);
            this.txtEnergyDemandMaximum.Name = "txtEnergyDemandMaximum";
            this.txtEnergyDemandMaximum.Size = new System.Drawing.Size(318, 31);
            this.txtEnergyDemandMaximum.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Axis Minimum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Axis Maximum";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnResetSolarGainsAxis);
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(648, 323);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Solar Gains per Window";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnResetSolarGainsAxis
            // 
            this.btnResetSolarGainsAxis.Location = new System.Drawing.Point(2, 98);
            this.btnResetSolarGainsAxis.Name = "btnResetSolarGainsAxis";
            this.btnResetSolarGainsAxis.Size = new System.Drawing.Size(158, 51);
            this.btnResetSolarGainsAxis.TabIndex = 2;
            this.btnResetSolarGainsAxis.Text = "Reset Axis";
            this.btnResetSolarGainsAxis.UseVisualStyleBackColor = true;
            this.btnResetSolarGainsAxis.Click += new System.EventHandler(this.btnResetSolarGainsAxis_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.txtSolarGainsMinimum, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtSolarGainsMaximum, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(644, 81);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // txtSolarGainsMinimum
            // 
            this.txtSolarGainsMinimum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSolarGainsMinimum.Location = new System.Drawing.Point(324, 2);
            this.txtSolarGainsMinimum.Margin = new System.Windows.Forms.Padding(2);
            this.txtSolarGainsMinimum.Name = "txtSolarGainsMinimum";
            this.txtSolarGainsMinimum.Size = new System.Drawing.Size(318, 31);
            this.txtSolarGainsMinimum.TabIndex = 0;
            // 
            // txtSolarGainsMaximum
            // 
            this.txtSolarGainsMaximum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSolarGainsMaximum.Location = new System.Drawing.Point(324, 42);
            this.txtSolarGainsMaximum.Margin = new System.Windows.Forms.Padding(2);
            this.txtSolarGainsMaximum.Name = "txtSolarGainsMaximum";
            this.txtSolarGainsMaximum.Size = new System.Drawing.Size(318, 31);
            this.txtSolarGainsMaximum.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Axis Minimum";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 40);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(151, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Axis Maximum";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 280);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(656, 81);
            this.panel1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(9, 20);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(158, 51);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(489, 20);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(158, 51);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // VisualizerPlotProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 361);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "VisualizerPlotProperties";
            this.Text = "Plot properties";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtEnergyDemandMinimum;
        private System.Windows.Forms.TextBox txtEnergyDemandMaximum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox txtSolarGainsMinimum;
        private System.Windows.Forms.TextBox txtSolarGainsMaximum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnResetDemandAxis;
        private System.Windows.Forms.Button btnResetSolarGainsAxis;
    }
}