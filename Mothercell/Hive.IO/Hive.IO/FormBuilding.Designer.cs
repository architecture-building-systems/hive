namespace Hive.IO
{
    partial class FormBuilding
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(77, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 42);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select Walls";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(77, 113);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 42);
            this.button2.TabIndex = 1;
            this.button2.Text = "Select Windows";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(77, 161);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(113, 42);
            this.button3.TabIndex = 2;
            this.button3.Text = "Select Roofs";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(77, 209);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(113, 42);
            this.button4.TabIndex = 3;
            this.button4.Text = "Select Floors";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(346, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(442, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Scrollable list-box showing all building components. tabs for windows, walls, roo" +
    "fs, ceiligns, ...";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(585, 252);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(181, 186);
            this.listBox1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(346, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "properties like u-value, density, cost, emssiosn, width, ...";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(357, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "check my old ifes TAS-to-SolarComputer tool";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(313, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(403, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Input are all building zones from rhino. This component solves for adjacencies n " +
    "stuff";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(196, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(420, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "clicking these buttons will jump to Rhino viewport, where you can select walls to" +
    " classify";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(249, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "e.g., set some walls to concrete, some to wood, etc";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(77, 257);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(113, 42);
            this.button5.TabIndex = 11;
            this.button5.Text = "Select Zones";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(77, 329);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(113, 42);
            this.button6.TabIndex = 12;
            this.button6.Text = "Set Schedules/Controls";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(253, 329);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(113, 42);
            this.button7.TabIndex = 13;
            this.button7.Text = "Set Energy System";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(77, 377);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(113, 42);
            this.button8.TabIndex = 14;
            this.button8.Text = "Set Internal Loads";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(250, 313);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(214, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "coupled to SolarSystems components smhw";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(181, 468);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(360, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "separate component for adjacent bldgs, policies, and other boundarie cond";
            // 
            // FormBuilding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 504);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "FormBuilding";
            this.Text = "FormBuilding";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}