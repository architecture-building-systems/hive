using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;


using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ProvingGround.Conduit.Classes;
using ProvingGround.Conduit.Utils;

namespace ProvingGround.Conduit
{
	public class nodeBubbleChart : GH_Component
	{
		#region Register Node


		/// <summary>
		/// Load Node Template
		/// </summary>
		public nodeBubbleChart()
			: base("Draw a bubble chart", "Bubble Chart", "Draw a bubble chart in the HUD", "Proving Ground", "HUD")
		{

		}

		/// <summary>
		/// Writes the value of "PersistentData" at each save of the GH document
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		//public override bool Write(GH_IWriter writer)
		//{

		//}

		/// <summary>
		/// Reads the value of "PersistentData" at each loading of the GH document
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		//public override bool Read(GH_IReader reader)
		//{

		//}

		//protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
		//{

		//}

		/// <summary>
		/// Component Exposure
		/// </summary>
		public override GH_Exposure Exposure
		{
			get { return GH_Exposure.secondary; }
		}

		/// <summary>
		/// GUID generator http://www.guidgenerator.com/online-guid-generator.aspx
		/// </summary>
		public override Guid ComponentGuid
		{
			get { return new Guid("bdbb5435-0f8e-4df4-8dda-c422dccc2d58"); }
		}
		#endregion

		#region Inputs/Outputs
		/// <summary>
		/// Node inputs
		/// </summary>
		/// <param name="pManager"></param>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{

			pManager.AddRectangleParameter("Boundary", "Bound", "Boundary for chart", GH_ParamAccess.item); // 0
			pManager.AddNumberParameter("Bubble Size", "Size", "Values for bubble size", GH_ParamAccess.list); // 1
			pManager.AddNumberParameter("Bubble Scale", "Scale", "Size of largest bubble, as percent relative to chart area (default is 0.05)", GH_ParamAccess.item, 0.05); // 2
			pManager.AddGenericParameter("Color Palette", "Palette", "Color palette to use for chart", GH_ParamAccess.item); // 3
			pManager.AddNumberParameter("X Values", "XVals", "X Axis Values", GH_ParamAccess.list); // 4
			pManager.AddTextParameter("X Axis Title", "XAxis", "X Axis Title", GH_ParamAccess.item, ""); // 5
			pManager.AddIntervalParameter("X axis range", "ValueRange", "Optional preset range for min/max values on the Axis", GH_ParamAccess.item); // opt 6
			pManager.AddGenericParameter("X Axis Style", "XStyle", "X Axis Style", GH_ParamAccess.item); // 7
			pManager.AddNumberParameter("Y Values", "YVals", "Y Axis Values", GH_ParamAccess.list); // 8
			pManager.AddTextParameter("Y Axis Title", "YAxis", "Y Axis Title", GH_ParamAccess.item, ""); // 9
			pManager.AddIntervalParameter("Y axis range", "ValueRange", "Optional preset range for min/max values on the Axis", GH_ParamAccess.item); // opt 10
			pManager.AddGenericParameter("YAxis Style", "YStyle", "Y Axis Style", GH_ParamAccess.item); // opt 11
			pManager.AddNumberParameter("Axis Padding", "AxisPad", "Padding for axis labels as pct of chart dimensions", GH_ParamAccess.item, 0.1); // 12
			pManager.AddGenericParameter("Chart Font", "Font", "Font for text objects in chart (leaving empty will use 'Default Chart Font')", GH_ParamAccess.item); // opt 13
			pManager.AddTextParameter("Labels", "Labels", "Optional list of labels for bubbles", GH_ParamAccess.list);
            pManager.AddNumberParameter("Label Size", "LableSz", "Multiplier for label size (relative to bubble size)", GH_ParamAccess.item, 0.3);

			pManager[3].Optional = true;

			pManager[6].Optional = true;
			pManager[7].Optional = true;

			pManager[10].Optional = true;
			pManager[11].Optional = true;

			pManager[13].Optional = true;
			pManager[14].Optional = true;

		}

		/// <summary>
		/// Node outputs
		/// </summary>
		/// <param name="pManager"></param>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.Register_GenericParam("HUD Drawing Object", "DrawObjs", "HUD Bubble Chart Objects");
		}
		#endregion

		#region Solution
		/// <summary>
		/// Code by the component
		/// </summary>
		/// <param name="DA"></param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{

			Rectangle3d B = new Rectangle3d();
			DA.GetData(0, ref B);

			List<double> BSZ = new List<double>();
			DA.GetDataList(1, BSZ);

			double BSC = 0.05;
			DA.GetData(2, ref BSC);

			clsPaletteStyle m_thisPalette = new clsPaletteStyle() { styleName = "Default Palette", unset = true };

			if (m_thisPalette.unset)
			{

				object m_objPalette = new object();
				DA.GetData(3, ref m_objPalette);

				if (m_objPalette.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
				{
					Grasshopper.Kernel.Types.GH_String m_paletteName = (Grasshopper.Kernel.Types.GH_String)m_objPalette;
					if (m_paletteName.IsValid)
					{
						m_thisPalette = new clsPaletteStyle() { styleName = m_paletteName.Value, unset = true };
					}
				}
				else
				{
					try
					{
						DA.GetData(3, ref m_thisPalette);
					}
					catch { }
				}
			}

			List<double> XV = new List<double>();
			DA.GetDataList(4, XV);

			string XAT = "";
			DA.GetData(5, ref XAT);

			Interval m_xValueRange = new Interval(0, 0);
			if (!DA.GetData(6, ref m_xValueRange)) m_xValueRange = new Interval(0, 0);

			clsCurveStyle m_xAxis = new clsCurveStyle() { styleName = "Default Axis", unset = true };

			if (m_xAxis.unset)
			{

				object m_objXAxis = new Object();
				DA.GetData(7, ref m_objXAxis);

				if (m_objXAxis.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
				{
					Grasshopper.Kernel.Types.GH_String m_catAxisName = (Grasshopper.Kernel.Types.GH_String)m_objXAxis;
					if (m_catAxisName.IsValid)
					{
						m_xAxis = new clsCurveStyle() { styleName = m_catAxisName.Value, unset = true };
					}
				}
				else
				{
					try
					{
						DA.GetData(7, ref m_xAxis);
					}
					catch { }
				}

			}

			List<double> YV = new List<double>();
			DA.GetDataList(8, YV);

			string YAT = "";
			DA.GetData(9, ref YAT);

			Interval m_yValueRange = new Interval(0, 0);
			if (!DA.GetData(10, ref m_yValueRange)) m_yValueRange = new Interval(0, 0);

			clsCurveStyle m_yAxis = new clsCurveStyle() { styleName = "Default Axis", unset = true };

			if (m_xAxis.unset)
			{

				object m_objYAxis = new Object();
				DA.GetData(11, ref m_objYAxis);

				if (m_objYAxis.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
				{
					Grasshopper.Kernel.Types.GH_String m_catAxisName = (Grasshopper.Kernel.Types.GH_String)m_objYAxis;
					if (m_catAxisName.IsValid)
					{
						m_yAxis = new clsCurveStyle() { styleName = m_catAxisName.Value, unset = true };
					}
				}
				else
				{
					try
					{
						DA.GetData(11, ref m_yAxis);
					}
					catch { }
				}

			}

			double AP = 0;
			DA.GetData(12, ref AP);

			clsFontStyle m_chartFont = new clsFontStyle() { styleName = "Default Font", unset = true }; // = new clsFontStyle() { styleName = "Default Chart Font", unset = true };

			if (m_chartFont.unset)
			{
				object ChtFnt = new object();
				DA.GetData(13, ref ChtFnt);

				if (ChtFnt.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
				{
					Grasshopper.Kernel.Types.GH_String m_fontName = (Grasshopper.Kernel.Types.GH_String)ChtFnt;
					if (m_fontName.IsValid)
					{
						m_chartFont = new clsFontStyle() { styleName = m_fontName.Value, unset = true };
					}
				}
				else
				{
					try
					{
						DA.GetData(13, ref m_chartFont);
					}
					catch { }
				}
			}

			// Labels
			List<string> L = new List<string>();
			DA.GetDataList(14, L);

            double m_labelSize = 0.3;
            DA.GetData(15, ref m_labelSize);

			// Main Operations


			if (BSZ.Count != XV.Count || XV.Count != YV.Count)
			{
				this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please ensure that bubble size, x value and y value lists are of equal length.");
				return;
			}

			if (BSC <= 0)
			{
				this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please ensure that the bubble scale is greater than zero.");	 
			}

			double m_bubbleMax = BSZ[0];

			double m_xMin = XV[0];
			double m_xMax = XV[0];
			double m_yMin = YV[0];
			double m_yMax = YV[0];

			for (int i = 0; i < BSZ.Count; i++)
			{

				m_bubbleMax = Math.Max(m_bubbleMax, BSZ[i]);

				m_xMin = Math.Min(m_xMin, XV[i]);
				m_xMax = Math.Max(m_xMax, XV[i]);

				m_yMin = Math.Min(m_yMin, YV[i]);
				m_yMax = Math.Max(m_yMax, YV[i]);

			}

            bool m_xRangeSpecified = (m_xValueRange.Length > 0);
            bool m_yRangeSpecified = (m_yValueRange.Length > 0);

			// get the interval of the value axess
			if (!m_xRangeSpecified) m_xValueRange = new Interval(m_xMin, m_xMax);
			if (!m_yRangeSpecified) m_yValueRange = new Interval(m_yMin, m_yMax);

            // get the interval of the value axis if it is not set
            double m_xIntersectValue = m_yMin < 0 && !m_yRangeSpecified ? m_yValueRange.NormalizedParameterAt(0) : 0;
            double m_yIntersectValue = m_xMin < 0 && !m_xRangeSpecified ? m_xValueRange.NormalizedParameterAt(0) : 0;

			// get the maximum dimension in either the X or Y direction
			double maxXY = Math.Max(B.X.Length, B.Y.Length);
			double minXY = Math.Min(B.X.Length, B.Y.Length);

			double adjX = 0;
			double adjY = 0;

			List<iDrawObject> chartObjects = new List<iDrawObject>();

			Plane basePlane = Plane.WorldXY;
			basePlane.Origin = B.Corner(0);

			// Set the Axis label values
			if (YAT != "")
			{
				adjX += AP * maxXY;
			}
			if (XAT != "")
			{
				adjY += AP * maxXY;
			}

			//double xAxisAdjust = VR.NormalizedParameterAt(0) * (B.Corner(3).Y - (B.Corner(0).Y + adjY));

			//create the X Axis Label

            if (XAT != "")
            {
                chartObjects.Add(new DrawText(new List<string>() { XAT },
                    new Rectangle3d(basePlane, new Point3d(B.Corner(0).X + adjX, B.Corner(0).Y, 0), new Point3d(B.Corner(1).X, B.Corner(0).Y + adjY * 0.8, 0)),
                    0, 0, new List<System.Drawing.Color>(), false, new clsFontStyle[] { m_chartFont }) as iDrawObject);
            }

            // create the Y Axis Label
            if (YAT != "")
            {
                chartObjects.Add(new DrawText(new List<string>() { YAT },
                    new Rectangle3d(basePlane, new Point3d(B.Corner(0).X, B.Corner(0).Y + adjY, 0), new Point3d(B.Corner(0).X + adjX * 0.8, B.Corner(3).Y, 0)),
                    0, 0, new List<System.Drawing.Color>(), true, new clsFontStyle[] { m_chartFont }) as iDrawObject);
            }

			// create the value axis

			Interval yRange = new Interval(B.Corner(0).Y + adjY , B.Corner(3).Y );
			Interval xRange = new Interval(B.Corner(0).X + adjX, B.Corner(1).X );

			// create the category axis
			if (m_xAxis.styleName != "None")
			{
				Line xAxis = new Line(new Point3d(B.Corner(0).X + adjX, yRange.ParameterAt(m_xIntersectValue), 0), new Point3d(B.Corner(1).X, yRange.ParameterAt(m_xIntersectValue), 0));
				chartObjects.Add(new DrawLines(new List<Line>() { xAxis }, new List<int>(), new List<Color>(), new clsCurveStyle[] { m_xAxis }, 3.0) as iDrawObject);
			}

			// create the category axis
			if (m_yAxis.styleName != "None")
			{
				Line yAxis = new Line(new Point3d(xRange.ParameterAt(m_yIntersectValue), B.Corner(0).Y + adjY, 0), new Point3d(xRange.ParameterAt(m_yIntersectValue), B.Corner(3).Y, 0));
				chartObjects.Add(new DrawLines(new List<Line>() { yAxis }, new List<int>(), new List<Color>(), new clsCurveStyle[] { m_yAxis }, 3.0) as iDrawObject);
			}

			double yBase = B.Corner(0).Y + adjY;
			double xBase = B.Corner(0).X + adjX;
			double maxRadius = BSC * minXY;
            double labelSize = m_labelSize * maxRadius;

			var m_meshList = new List<Mesh>();

			for (int i = 0; i < BSZ.Count; i++)
			{

				double xSet = xRange.ParameterAt(m_xValueRange.NormalizedParameterAt(XV[i]));
				double ySet = yRange.ParameterAt(m_yValueRange.NormalizedParameterAt(YV[i]));
				double radius = m_bubbleMax > 0 ? maxRadius * (BSZ[i] / m_bubbleMax) : maxRadius;
                

				Circle m_thisBubble = new Circle(new Point3d(xSet, ySet, 0), radius);
				m_meshList.Add(Mesh.CreateFromBrep(Brep.CreatePlanarBreps(m_thisBubble.ToNurbsCurve())[0])[0]);

                if(i < L.Count)
                {                   
                    chartObjects.Add(new DrawText(new List<string>() { L[i] }, new Rectangle3d(basePlane, new Point3d(xSet-labelSize, ySet-labelSize, 0), new Point3d(xSet + labelSize, ySet + labelSize,0)),
                        0, 0, new List<System.Drawing.Color>(), false, new clsFontStyle[] { m_chartFont }) as iDrawObject);
                }

			}
			 
			chartObjects.Add(new DrawMeshGroup(m_meshList, 1.0, m_thisPalette, true));

			DA.SetDataList(0, chartObjects);
		}
		#endregion
	}
}


