using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Hive.IO.Forms.Controls
{
    public partial class SurfaceTechnologyProperties : ConversionTechPropertiesBase
    {
        private bool _initializingControls;

        public SurfaceTechnologyProperties()
        {
            InitializeComponent();
        }

        public override ConversionTechPropertiesViewModel Conversion
        {
            get => base.Conversion;
            set
            {
                base.Conversion = value;
                lblDescription.Text = Conversion.ModuleType.Description;
                technologyImage.Image = Conversion.TechnologyImage;

                UpdateAvailableSurfaces();
                UpdateModuleTypesList();
                UpdateCalculatedFields();
            }
        }

        /// <summary>
        ///     See [here](https://stackoverflow.com/a/3165330/2260) for why I'm not
        ///     hooking the event up directly with the handler defined in the base class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private new void Validating(object sender, CancelEventArgs e)
        {
            TextBox_Validating(sender, e);

            UpdateCalculatedFields();
        }

        private void UpdateAvailableSurfaces()
        {
            try
            {
                _initializingControls = true;

                lstAvailableSurfaces.Items.Clear();
                lstAvailableSurfaces.Items.AddRange(Conversion.AvailableSurfaces.ToArray<object>());

                for (var i = 0; i < lstAvailableSurfaces.Items.Count; i++)
                {
                    lstAvailableSurfaces.SetSelected(i,
                        Conversion.SelectedSurfaces.Contains(lstAvailableSurfaces.Items[i]));
                }
            }
            finally
            {
                _initializingControls = false;
            }
        }

        private void UpdateModuleTypesList()
        {
            try
            {
                _initializingControls = true;
                cboModuleType.Items.Clear();
                cboModuleType.Items.AddRange(
                    Conversion.ModuleTypes.Select(mt => mt.Name).ToArray<object>());
                cboModuleType.SelectedItem = Conversion.ModuleType.Name;
            }
            finally
            {
                _initializingControls = false;
            }



        }

        private void cboModuleType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (_initializingControls) return;

            var moduleName = cboModuleType.SelectedItem.ToString();
            var moduleType = Conversion.ModuleTypes.First(mt => mt.Name == moduleName);
            Conversion.ModuleType = moduleType;

            lblDescription.Text = Conversion.ModuleType.Description;
            technologyImage.Image = Conversion.TechnologyImage;


            foreach (var textBox in GetAll(this, typeof(TextBox)).Cast<TextBox>()) UpdateTextBoxText(textBox);

        }


        private void lstAvailableSurfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializingControls)
            {
                return;
            }

            Conversion.SelectedSurfaces = new List<SurfaceViewModel>(lstAvailableSurfaces.SelectedItems.Cast<SurfaceViewModel>());
            UpdateCalculatedFields();
        }

        private void lstAvailableSurfaces_SelectAll(object sender, KeyEventArgs e)
        {
            if (_initializingControls)
            {
                return;
            }

            if (e.Control && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < lstAvailableSurfaces.Items.Count; i++)
                {
                    lstAvailableSurfaces.SetSelected(i, true);
                }

                Conversion.SelectedSurfaces = new List<SurfaceViewModel>(lstAvailableSurfaces.SelectedItems.Cast<SurfaceViewModel>());
                UpdateCalculatedFields();
            }
        }

        private void UpdateCalculatedFields()
        {
            txtCapacity.Text = $"{Conversion.SurfaceTechCapacity:0.00}";
            txtEmbodiedEmissions.Text = $"{Conversion.EmbodiedEmissions:0.0}";
            txtCapitalCost.Text = $"{Conversion.CapitalCost:0.00}";
            txtArea.Text = $"{Conversion.Area:0.00}";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.bipv.ch");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("http://explorerise.com");
        }

        private void cboModuleType_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cboModuleType, cboModuleType.SelectedItem.ToString());
        }



        // create a Custom Control, derived from ComboBox, override its WndProc method to create a custom ListBox that fits the screen and 
        // make additional events on Hover over each combobox item
        public partial class JComboBox : ComboBox
        {

            public JComboBox()
            {

            }

            public JComboBox(IContainer container)
                : this()
            {
                container.Add(this);
            }

            protected override void OnDropDown(EventArgs e)
            {
                base.OnDropDown(e);
                int width = DropDownWidth;
                Graphics g = CreateGraphics();
                Font font = Font;
                int vertScrollBarWidth =
                    (Items.Count > MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth : 0;

                int newWidth = 0;
                foreach (var s in Items)
                {
                    if (s is string)
                        newWidth = (int)g.MeasureString((string)s, font).Width + vertScrollBarWidth;
                    if (width < newWidth)
                    {
                        width = newWidth;
                    }
                }
                DropDownWidth = width;
            }


            private int yPos = 0;
            private int xPos = 0;
            private int scrollPos = 0;
            private int xFactor = -1;
            private int simpleOffset = 0;

            // Import the GetScrollInfo function from user32.dll
            [DllImport("user32.dll", SetLastError = true)]
            private static extern int GetScrollInfo(IntPtr hWnd, int n, ref ScrollInfoStruct lpScrollInfo);

            // Win32 constants
            private const int SB_VERT = 1;
            private const int SIF_TRACKPOS = 0x10;
            private const int SIF_RANGE = 0x1;
            private const int SIF_POS = 0x4;
            private const int SIF_PAGE = 0x2;
            private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;

            private const int SCROLLBAR_WIDTH = 17;
            private const int LISTBOX_YOFFSET = 21;

            // Return structure for the GetScrollInfo method
            [StructLayout(LayoutKind.Sequential)]
            private struct ScrollInfoStruct
            {
                public int cbSize;
                public int fMask;
                public int nMin;
                public int nMax;
                public int nPage;
                public int nPos;
                public int nTrackPos;
            }

            public event HoverEventHandler Hover;

            protected virtual void OnHover(HoverEventArgs e)
            {
                HoverEventHandler handler = Hover;
                if (handler != null)
                {
                    // Invokes the delegates. 
                    handler(this, e);
                }
            }

            //Capture messages coming to our combobox
            protected override void WndProc(ref Message msg)
            {
                //This message code indicates the value in the list is changing
                //32 is for DropDownStyle == Simple            
                if ((msg.Msg == 308) || (msg.Msg == 32) || (msg.Msg == 533))
                {
                    int onScreenIndex = 0;

                    // Get the mouse position relative to this control
                    Point LocalMousePosition = PointToClient(Cursor.Position);
                    xPos = LocalMousePosition.X;

                    if (this.DropDownStyle == ComboBoxStyle.Simple)
                    {
                        yPos = LocalMousePosition.Y - (this.ItemHeight + 10);
                    }
                    else
                    {
                        yPos = LocalMousePosition.Y - this.Size.Height - 1;
                    }
                    // awoid problem with first item
                    if (msg.Msg == 533)
                    {
                        yPos = 0;
                    }
                    // save our y position which we need to ensure the cursor is
                    // inside the drop down list for updating purposes
                    int oldYPos = yPos;

                    // get the 0-based index of where the cursor is on screen
                    // as if it were inside the listbox
                    while (yPos >= this.ItemHeight)
                    {
                        yPos -= this.ItemHeight;
                        onScreenIndex++;
                    }

                    //if (yPos < 0) { onScreenIndex = -1; }
                    ScrollInfoStruct si = new ScrollInfoStruct();
                    si.fMask = SIF_ALL;
                    si.cbSize = Marshal.SizeOf(si);
                    // msg.LParam holds the hWnd to the drop down list that appears
                    int getScrollInfoResult = 0;
                    getScrollInfoResult = GetScrollInfo(msg.LParam, SB_VERT, ref si);

                    // k returns 0 on error, so if there is no error add the current
                    // track position of the scrollbar to our index
                    if (getScrollInfoResult > 0)
                    {
                        onScreenIndex += si.nTrackPos;

                        if (this.DropDownStyle == ComboBoxStyle.Simple)
                        {
                            simpleOffset = si.nTrackPos;
                        }
                    }

                    // Add our offset modifier if we're a simple combobox since we don't
                    // continuously receive scrollbar information in this mode.
                    // Then make sure the item we're previewing is actually on screen.
                    if (this.DropDownStyle == ComboBoxStyle.Simple)
                    {
                        onScreenIndex += simpleOffset;
                        if (onScreenIndex > ((this.DropDownHeight / this.ItemHeight) + simpleOffset))
                        {
                            onScreenIndex = ((this.DropDownHeight / this.ItemHeight) + simpleOffset - 1);
                        }
                    }

                    // Check we're actually inside the drop down window that appears and 
                    // not just over its scrollbar before we actually try to update anything
                    // then if we are raise the Hover event for this comboBox
                    if (!(xPos > this.DropDownWidth - SCROLLBAR_WIDTH || xPos < 1 || oldYPos < 0))
                    {
                        HoverEventArgs e = new HoverEventArgs();
                        e.Rectangle = new Rectangle(0, ((onScreenIndex - si.nTrackPos) * this.ItemHeight) + this.Height, this.DropDownWidth, this.ItemHeight);
                        e.ItemIndex = (onScreenIndex > this.Items.Count - 1) ? this.Items.Count - 1 : onScreenIndex;
                        OnHover(e);
                        // if scrollPos doesn't equal the nPos from our ScrollInfoStruct then
                        // the mousewheel was most likely used to scroll the drop down list
                        // while the mouse was inside it - this means we have to manually
                        // tell the drop down to repaint itself to update where it is hovering
                        // still posible to "outscroll" this method but it works better than
                        // without it present
                        if (scrollPos != si.nPos)
                        {
                            Cursor.Position = new Point(Cursor.Position.X + xFactor, Cursor.Position.Y);
                            xFactor = -xFactor;
                        }
                    }
                    scrollPos = si.nPos;
                }
                // Pass on our message
                base.WndProc(ref msg);
            }
        }

        /// <summary>
        /// Class that contains data for the hover event 
        /// </summary>
        public class HoverEventArgs : EventArgs
        {
            private int _itemIndex = 0;
            private Rectangle _rectangle;
            public int ItemIndex
            {
                get
                {
                    return _itemIndex;
                }
                set
                {
                    _itemIndex = value;
                }
            }
            public Rectangle Rectangle
            {
                get
                {
                    return _rectangle;
                }
                set
                {
                    _rectangle = value;
                }
            }
        }

        /// <summary>
        /// Delegate declaration 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void HoverEventHandler(object sender, HoverEventArgs e);

    }

}
