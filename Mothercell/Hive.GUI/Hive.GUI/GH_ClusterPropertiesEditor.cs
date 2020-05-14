using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Hive.GUI
{
    class GH_ClusterPropertiesEditor : Form
    {
        private IContainer components;

        public GH_ClusterPropertiesEditor()
        {
            this.Load += new EventHandler(this.GH_ClusterPropertiesEditor_Load);
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing || this.components == null)
                    return;
                this.components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.pnlButtons = new Panel();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.grpCluster = new GroupBox();
            this.tblCluster = new TableLayoutPanel();
            this.txtDescription = new TextBox();
            this.lblDescription = new Label();
            this.txtNickName = new TextBox();
            this.lblNickName = new Label();
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.iconPicker = new GH_IconPicker();
            this.lblIcon = new Label();
            this.Panel1 = new Panel();
            this.grpAuthor = new GroupBox();
            this.TableLayoutPanel1 = new TableLayoutPanel();
            this.txtPhone = new TextBox();
            this.txtEMail = new TextBox();
            this.txtWebsite = new TextBox();
            this.Label3 = new Label();
            this.Label2 = new Label();
            this.Label1 = new Label();
            this.txtAddress = new TextBox();
            this.AddressLabel = new Label();
            this.txtCopyright = new TextBox();
            this.lblCopyright = new Label();
            this.txtCompany = new TextBox();
            this.lblCompany = new Label();
            this.lblAuthor = new Label();
            this.txtAuthor = new TextBox();
            this.pnlButtons.SuspendLayout();
            this.grpCluster.SuspendLayout();
            this.tblCluster.SuspendLayout();
            this.grpAuthor.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            this.pnlButtons.Controls.Add((Control)this.btnOK);
            this.pnlButtons.Controls.Add((Control)this.btnCancel);
            this.pnlButtons.Dock = DockStyle.Bottom;
            this.pnlButtons.Location = new Point(2, 395);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new Padding(0, 8, 0, 0);
            this.pnlButtons.Size = new Size(300, 32);
            this.pnlButtons.TabIndex = 0;
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Dock = DockStyle.Right;
            this.btnOK.Location = new Point(140, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(80, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Dock = DockStyle.Right;
            this.btnCancel.Location = new Point(220, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(80, 24);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.grpCluster.Controls.Add((Control)this.tblCluster);
            this.grpCluster.Dock = DockStyle.Top;
            this.grpCluster.Location = new Point(2, 6);
            this.grpCluster.Name = "grpCluster";
            this.grpCluster.Size = new Size(300, 175);
            this.grpCluster.TabIndex = 1;
            this.grpCluster.TabStop = false;
            this.grpCluster.Text = "Cluster";
            this.tblCluster.ColumnCount = 2;
            this.tblCluster.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
            this.tblCluster.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tblCluster.Controls.Add((Control)this.txtDescription, 1, 2);
            this.tblCluster.Controls.Add((Control)this.lblDescription, 0, 2);
            this.tblCluster.Controls.Add((Control)this.txtNickName, 1, 1);
            this.tblCluster.Controls.Add((Control)this.lblNickName, 0, 1);
            this.tblCluster.Controls.Add((Control)this.lblName, 0, 0);
            this.tblCluster.Controls.Add((Control)this.txtName, 1, 0);
            this.tblCluster.Controls.Add((Control)this.iconPicker, 1, 3);
            this.tblCluster.Controls.Add((Control)this.lblIcon, 0, 3);
            this.tblCluster.Dock = DockStyle.Fill;
            this.tblCluster.Location = new Point(3, 16);
            this.tblCluster.Name = "tblCluster";
            this.tblCluster.RowCount = 4;
            this.tblCluster.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.tblCluster.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.tblCluster.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tblCluster.RowStyles.Add(new RowStyle(SizeType.Absolute, 32f));
            this.tblCluster.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
            this.tblCluster.Size = new Size(294, 156);
            this.tblCluster.TabIndex = 0;
            this.txtDescription.Dock = DockStyle.Fill;
            this.txtDescription.Location = new Point(80, 44);
            this.txtDescription.Margin = new Padding(0);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new Size(214, 80);
            this.txtDescription.TabIndex = 5;
            this.txtDescription.Text = "<Description>";
            this.lblDescription.Dock = DockStyle.Fill;
            this.lblDescription.Location = new Point(1, 45);
            this.lblDescription.Margin = new Padding(1, 1, 1, 1);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new Size(78, 78);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Description";
            this.lblDescription.TextAlign = ContentAlignment.MiddleRight;
            this.txtNickName.Dock = DockStyle.Fill;
            this.txtNickName.Location = new Point(80, 22);
            this.txtNickName.Margin = new Padding(0);
            this.txtNickName.Name = "txtNickName";
            this.txtNickName.Size = new Size(214, 20);
            this.txtNickName.TabIndex = 3;
            this.txtNickName.Text = "<NickName>";
            this.lblNickName.Dock = DockStyle.Fill;
            this.lblNickName.Location = new Point(1, 23);
            this.lblNickName.Margin = new Padding(1, 1, 1, 1);
            this.lblNickName.Name = "lblNickName";
            this.lblNickName.Size = new Size(78, 20);
            this.lblNickName.TabIndex = 2;
            this.lblNickName.Text = "NickName";
            this.lblNickName.TextAlign = ContentAlignment.MiddleRight;
            this.lblName.Dock = DockStyle.Fill;
            this.lblName.Location = new Point(1, 1);
            this.lblName.Margin = new Padding(1, 1, 1, 1);
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(78, 20);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            this.lblName.TextAlign = ContentAlignment.MiddleRight;
            this.txtName.Dock = DockStyle.Fill;
            this.txtName.Location = new Point(80, 0);
            this.txtName.Margin = new Padding(0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new Size(214, 20);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "<Name>";
            this.iconPicker.AllowDrop = true;
            this.iconPicker.BackColor = SystemColors.Window;
            this.iconPicker.BorderStyle = BorderStyle.Fixed3D;
            this.iconPicker.Dock = DockStyle.Fill;
            this.iconPicker.Icon = (Bitmap)null;
            this.iconPicker.Location = new Point(80, 124);
            this.iconPicker.Margin = new Padding(0);
            this.iconPicker.Name = "iconPicker";
            this.iconPicker.Size = new Size(214, 32);
            this.iconPicker.TabIndex = 7;
            this.lblIcon.Dock = DockStyle.Fill;
            this.lblIcon.Location = new Point(1, 125);
            this.lblIcon.Margin = new Padding(1, 1, 1, 1);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new Size(78, 30);
            this.lblIcon.TabIndex = 6;
            this.lblIcon.Text = "Icon";
            this.lblIcon.TextAlign = ContentAlignment.MiddleRight;
            this.Panel1.Dock = DockStyle.Top;
            this.Panel1.Location = new Point(2, 181);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new Size(300, 10);
            this.Panel1.TabIndex = 2;
            this.grpAuthor.Controls.Add((Control)this.TableLayoutPanel1);
            this.grpAuthor.Dock = DockStyle.Fill;
            this.grpAuthor.Location = new Point(2, 191);
            this.grpAuthor.Name = "grpAuthor";
            this.grpAuthor.Size = new Size(300, 204);
            this.grpAuthor.TabIndex = 3;
            this.grpAuthor.TabStop = false;
            this.grpAuthor.Text = "Author";
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.Controls.Add((Control)this.txtPhone, 1, 6);
            this.TableLayoutPanel1.Controls.Add((Control)this.txtEMail, 1, 5);
            this.TableLayoutPanel1.Controls.Add((Control)this.txtWebsite, 1, 4);
            this.TableLayoutPanel1.Controls.Add((Control)this.Label3, 0, 6);
            this.TableLayoutPanel1.Controls.Add((Control)this.Label2, 0, 5);
            this.TableLayoutPanel1.Controls.Add((Control)this.Label1, 0, 4);
            this.TableLayoutPanel1.Controls.Add((Control)this.txtAddress, 1, 3);
            this.TableLayoutPanel1.Controls.Add((Control)this.AddressLabel, 0, 3);
            this.TableLayoutPanel1.Controls.Add((Control)this.txtCopyright, 1, 2);
            this.TableLayoutPanel1.Controls.Add((Control)this.lblCopyright, 0, 2);
            this.TableLayoutPanel1.Controls.Add((Control)this.txtCompany, 1, 1);
            this.TableLayoutPanel1.Controls.Add((Control)this.lblCompany, 0, 1);
            this.TableLayoutPanel1.Controls.Add((Control)this.lblAuthor, 0, 0);
            this.TableLayoutPanel1.Controls.Add((Control)this.txtAuthor, 1, 0);
            this.TableLayoutPanel1.Dock = DockStyle.Fill;
            this.TableLayoutPanel1.Location = new Point(3, 16);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 7;
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.TableLayoutPanel1.Size = new Size(294, 185);
            this.TableLayoutPanel1.TabIndex = 1;
            this.txtPhone.Dock = DockStyle.Fill;
            this.txtPhone.Location = new Point(80, 163);
            this.txtPhone.Margin = new Padding(0);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new Size(214, 20);
            this.txtPhone.TabIndex = 13;
            this.txtPhone.Text = "<Phone>";
            this.txtEMail.Dock = DockStyle.Fill;
            this.txtEMail.Location = new Point(80, 141);
            this.txtEMail.Margin = new Padding(0);
            this.txtEMail.Name = "txtEMail";
            this.txtEMail.Size = new Size(214, 20);
            this.txtEMail.TabIndex = 12;
            this.txtEMail.Text = "<E-Mail>";
            this.txtWebsite.Dock = DockStyle.Fill;
            this.txtWebsite.Location = new Point(80, 119);
            this.txtWebsite.Margin = new Padding(0);
            this.txtWebsite.Name = "txtWebsite";
            this.txtWebsite.Size = new Size(214, 20);
            this.txtWebsite.TabIndex = 11;
            this.txtWebsite.Text = "<Website>";
            this.Label3.Dock = DockStyle.Fill;
            this.Label3.Location = new Point(1, 164);
            this.Label3.Margin = new Padding(1, 1, 1, 1);
            this.Label3.Name = "Label3";
            this.Label3.Size = new Size(78, 20);
            this.Label3.TabIndex = 10;
            this.Label3.Text = "Phone";
            this.Label3.TextAlign = ContentAlignment.MiddleRight;
            this.Label2.Dock = DockStyle.Fill;
            this.Label2.Location = new Point(1, 142);
            this.Label2.Margin = new Padding(1, 1, 1, 1);
            this.Label2.Name = "Label2";
            this.Label2.Size = new Size(78, 20);
            this.Label2.TabIndex = 9;
            this.Label2.Text = "E-Mail";
            this.Label2.TextAlign = ContentAlignment.MiddleRight;
            this.Label1.Dock = DockStyle.Fill;
            this.Label1.Location = new Point(1, 120);
            this.Label1.Margin = new Padding(1, 1, 1, 1);
            this.Label1.Name = "Label1";
            this.Label1.Size = new Size(78, 20);
            this.Label1.TabIndex = 8;
            this.Label1.Text = "Website";
            this.Label1.TextAlign = ContentAlignment.MiddleRight;
            this.txtAddress.Dock = DockStyle.Fill;
            this.txtAddress.Location = new Point(80, 66);
            this.txtAddress.Margin = new Padding(0);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new Size(214, 53);
            this.txtAddress.TabIndex = 7;
            this.txtAddress.Text = "<Address>";
            this.AddressLabel.Dock = DockStyle.Fill;
            this.AddressLabel.Location = new Point(1, 67);
            this.AddressLabel.Margin = new Padding(1, 1, 1, 1);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new Size(78, 51);
            this.AddressLabel.TabIndex = 6;
            this.AddressLabel.Text = "Address";
            this.AddressLabel.TextAlign = ContentAlignment.MiddleRight;
            this.txtCopyright.Dock = DockStyle.Fill;
            this.txtCopyright.Location = new Point(80, 44);
            this.txtCopyright.Margin = new Padding(0);
            this.txtCopyright.Name = "txtCopyright";
            this.txtCopyright.Size = new Size(214, 20);
            this.txtCopyright.TabIndex = 5;
            this.txtCopyright.Text = "<Copyright>";
            this.lblCopyright.Dock = DockStyle.Fill;
            this.lblCopyright.Location = new Point(1, 45);
            this.lblCopyright.Margin = new Padding(1, 1, 1, 1);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new Size(78, 20);
            this.lblCopyright.TabIndex = 4;
            this.lblCopyright.Text = "Copyright";
            this.lblCopyright.TextAlign = ContentAlignment.MiddleRight;
            this.txtCompany.Dock = DockStyle.Fill;
            this.txtCompany.Location = new Point(80, 22);
            this.txtCompany.Margin = new Padding(0);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new Size(214, 20);
            this.txtCompany.TabIndex = 3;
            this.txtCompany.Text = "<Company>";
            this.lblCompany.Dock = DockStyle.Fill;
            this.lblCompany.Location = new Point(1, 23);
            this.lblCompany.Margin = new Padding(1, 1, 1, 1);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new Size(78, 20);
            this.lblCompany.TabIndex = 2;
            this.lblCompany.Text = "Company";
            this.lblCompany.TextAlign = ContentAlignment.MiddleRight;
            this.lblAuthor.Dock = DockStyle.Fill;
            this.lblAuthor.Location = new Point(1, 1);
            this.lblAuthor.Margin = new Padding(1, 1, 1, 1);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new Size(78, 20);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "Author";
            this.lblAuthor.TextAlign = ContentAlignment.MiddleRight;
            this.txtAuthor.Dock = DockStyle.Fill;
            this.txtAuthor.Location = new Point(80, 0);
            this.txtAuthor.Margin = new Padding(0);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new Size(214, 20);
            this.txtAuthor.TabIndex = 1;
            this.txtAuthor.Text = "<Author>";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(304, 429);
            this.Controls.Add((Control)this.grpAuthor);
            this.Controls.Add((Control)this.Panel1);
            this.Controls.Add((Control)this.grpCluster);
            this.Controls.Add((Control)this.pnlButtons);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(245, 435);
            this.Name = nameof(GH_ClusterPropertiesEditor);
            this.Padding = new Padding(2, 6, 2, 2);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.StartPosition = FormStartPosition.Manual;
            this.Text = "Cluster Properties";
            this.pnlButtons.ResumeLayout(false);
            this.grpCluster.ResumeLayout(false);
            this.tblCluster.ResumeLayout(false);
            this.tblCluster.PerformLayout();
            this.grpAuthor.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        internal virtual Panel pnlButtons { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Button btnOK { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Button btnCancel { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual GroupBox grpCluster { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Panel Panel1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual GroupBox grpAuthor { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TableLayoutPanel tblCluster { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label lblName { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtName { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtNickName { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label lblNickName { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label lblDescription { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtDescription { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label lblIcon { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual GH_IconPicker iconPicker { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TableLayoutPanel TableLayoutPanel1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label AddressLabel { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtCopyright
        {
            get
            {
                return this.txtCopyright;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler eventHandler = new EventHandler(this.txtCopyright_LostFocus);
                TextBox txtCopyright1 = this.txtCopyright;
                if (txtCopyright1 != null)
                    txtCopyright1.LostFocus -= eventHandler;
                this.txtCopyright = value;
                TextBox txtCopyright2 = this.txtCopyright;
                if (txtCopyright2 == null)
                    return;
                txtCopyright2.LostFocus += eventHandler;
            }
        }

        internal virtual Label lblCopyright { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtCompany { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label lblCompany { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label lblAuthor { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtAuthor { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtAddress { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtPhone { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtEMail { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual TextBox txtWebsite { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label Label3 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label Label2 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        internal virtual Label Label1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        private void GH_ClusterPropertiesEditor_Load(object sender, EventArgs e)
        {
            GH_WindowsControlUtil.FixTextRenderingDefault(this.Controls);
        }

        private void txtCopyright_LostFocus(object sender, EventArgs e)
        {
            this.txtCopyright.Text = this.txtCopyright.Text.Replace("(c)", "©");
            this.txtCopyright.Text = this.txtCopyright.Text.Replace("(C)", "©");
            this.txtCopyright.Text = this.txtCopyright.Text.Replace("(r)", "®");
            this.txtCopyright.Text = this.txtCopyright.Text.Replace("(R)", "®");
            this.txtCopyright.Text = this.txtCopyright.Text.Replace("(tm)", "™");
            this.txtCopyright.Text = this.txtCopyright.Text.Replace("(TM)", "™");
        }

        public void LoadCluster(GH_Cluster_OBSOLETE cluster)
        {
            this.txtName.Text = cluster.Name;
            this.txtNickName.Text = cluster.NickName;
            this.txtDescription.Text = cluster.Description;
            this.iconPicker.Icon = cluster.Icon_24x24;
            this.txtAuthor.Text = cluster.AuthorName;
            this.txtCompany.Text = cluster.AuthorCompany;
            this.txtAddress.Text = cluster.AuthorContact;
            this.txtCopyright.Text = cluster.AuthorCopyright;
        }

        //public void SaveCluster(GH_Cluster_OBSOLETE cluster)
        //{
        //    cluster.Name = this.txtName.Text;
        //    cluster.NickName = this.txtNickName.Text;
        //    cluster.Description = this.txtDescription.Text;
        //    cluster.AuthorName = this.txtAuthor.Text;
        //    cluster.AuthorCompany = this.txtCompany.Text;
        //    cluster.AuthorContact = this.txtAddress.Text;
        //    cluster.AuthorCopyright = this.txtCopyright.Text;
        //    if (!this.iconPicker.IconChanged)
        //        return;
        //    cluster.SetIconOverride(this.iconPicker.Icon);
        //}

        public void LoadCluster(GH_Cluster cluster)
        {
            this.txtName.Text = cluster.Name;
            this.txtNickName.Text = cluster.NickName;
            this.txtDescription.Text = cluster.Description;
            this.iconPicker.Icon = cluster.Icon_24x24;
            this.txtAuthor.Text = cluster.Author.Name;
            this.txtCompany.Text = cluster.Author.Company;
            this.txtCopyright.Text = cluster.Author.Copyright;
            this.txtAddress.Text = cluster.Author.Address;
            this.txtWebsite.Text = cluster.Author.Website;
            this.txtEMail.Text = cluster.Author.Email;
            this.txtPhone.Text = cluster.Author.Phone;
        }

        public void WriteToCluster(GH_Cluster cluster)
        {
            cluster.Name = this.txtName.Text;
            cluster.NickName = this.txtNickName.Text;
            cluster.Description = this.txtDescription.Text;
            cluster.Author = (IGH_Author)new GH_Author()
            {
                Name = this.txtAuthor.Text,
                Company = this.txtCompany.Text,
                Copyright = this.txtCopyright.Text,
                Address = this.txtAddress.Text,
                Website = this.txtWebsite.Text,
                EMail = this.txtEMail.Text,
                Phone = this.txtPhone.Text
            };
            if (!this.iconPicker.IconChanged)
                return;
            cluster.SetIconOverride(this.iconPicker.Icon);
        }
    }
}