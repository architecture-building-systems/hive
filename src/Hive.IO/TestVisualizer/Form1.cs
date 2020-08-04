using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.GUI.Canvas;
using Hive.IO.GHComponents;

namespace TestVisualizer
{
    public partial class Form1 : Form
    {
        private GhVisualizerAttributes _attributes;

        public Form1()
        {
            InitializeComponent();
            var visualizer = new GhVisualizer();
            visualizer.CreateAttributes();
            _attributes = visualizer.Attributes as GhVisualizerAttributes;
            GhVisualizerAttributes.TitleBarHeight = 0;
            
            panel1.Paint += Panel1_Paint;
            panel1.Resize += Panel1_Resize;
        }

        private void Panel1_Resize(object sender, EventArgs e)
        {
            _attributes.Bounds = panel1.Bounds;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.White), Bounds);
            _attributes.Bounds = panel1.Bounds;
            _attributes.RenderPlot(e.Graphics);
        }
    }
}
