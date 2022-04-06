﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Hive.IO.Forms;
using Hive.IO.Plots;
using Hive.IO.Results;
using Rhino;

namespace Hive.IO.GhInputOutput
{
    public class GhVisualizerAttributes : GH_ResizableAttributes<GhVisualizer>
    {
        private const int Padding = 6;
        private static readonly float MinWidth = 3 * (TitleBarHeight + Padding) + 4 * (MenuButtonPanel.Spacer + MenuButtonPanel.SideLength);

        // 15 is chosen to fit the energy balance plot legend plus some space for the plot itself
        private static readonly float MinHeight =
            TitleBarHeight + 15 * GH_FontServer.MeasureString("abc", GH_FontServer.StandardBold).Height;

        private readonly KpiPlot[] _kpiPlots;

        private readonly PlotSelector _plotSelector = new PlotSelector();

        public GhVisualizerAttributes(GhVisualizer owner) : base(owner)
        {
            var energyKpiConfig = new KpiPlotProperties
            {
                Color = Color.FromArgb(225, 242, 31),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kWh",
                NormalizedUnitText = "kWh/m²",
                Data = (results, normalized) => results.TotalEnergy(normalized),
                Kpi = Kpi.Energy
            };
            var emissionsKpiConfig = new KpiPlotProperties
            {
                Color = Color.FromArgb(136, 219, 68),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "kgCO₂",
                NormalizedUnitText = "kgCO₂/m²",
                Data = (results, normalized) => results.TotalEmissions(normalized),
                Kpi = Kpi.Emissions
            };
            var costsKpiConfig = new KpiPlotProperties
            {
                Color = Color.FromArgb(222, 180, 109),
                BenchmarkFailedColor = Color.FromArgb(166, 78, 2),
                UnitText = "CHF",
                NormalizedUnitText = "CHF/m²",
                Data = (results, normalized) => results.TotalCosts(normalized),
                Kpi = Kpi.Costs
            };

            var costsKpi = new KpiPlot(costsKpiConfig);
            costsKpi.OnClicked += _plotSelector.CostsKpiClicked;

            var emissionsKpi = new KpiPlot(emissionsKpiConfig);
            emissionsKpi.OnClicked += _plotSelector.EmissionsKpiClicked;

            var energyKpi = new KpiPlot(energyKpiConfig);
            energyKpi.OnClicked += _plotSelector.EnergyKpiClicked;

            //bool Normalized = true;
            var mbNormalize = _plotSelector.Normalized ? new BlackMenuButton("/m²") : new MenuButton("/m²");
            mbNormalize.OnClicked += (sender, e) => _plotSelector.Normalized = !_plotSelector.Normalized;

            _kpiPlots = new KpiPlot[]
            {
                // from the right
                costsKpi,
                emissionsKpi,
                energyKpi,
                //mbNormalize
            };
        }

        // make sure we have a minimum size
        public static float TitleBarHeight =>
            GH_FontServer.MeasureString("1000", GH_FontServer.StandardBold).Height +
            3 * GH_FontServer.MeasureString("KPI", GH_FontServer.Standard).Height;

        // FIXME: what goes here?
        public override string PathName => "PathName_GHVisualizer";

        protected override Size MinimumSize => new Size(50, 50);

        protected override Padding SizingBorders => new Padding(Padding);

        private RectangleF InnerBounds => Bounds.CloneInflate(-Padding, -Padding);

        private List<string> AxisLimitPlots = new List<string>{ "Hive.IO.Plots.DemandMonthlyPlot", 
            "Hive.IO.Plots.DemandMonthlyNormalizedPlot", 
            "Hive.IO.Plots.SolarGainsPerWindowPlot",
            "Hive.IO.Plots.SolarGainsPerWindowNormalizedPlot"};

        private RectangleF PlotBounds
        {
            get
            {
                var plotBounds = InnerBounds;
                plotBounds.Height -= TitleBarHeight;
                plotBounds.Offset(0, TitleBarHeight);
                return plotBounds;
            }
        }

        private bool InYAXisBounds = false;
        private RectangleF YAxisBounds
        {
            get
            {
                var YAxisBounds = InnerBounds;
                YAxisBounds.Height -= TitleBarHeight + 55;
                YAxisBounds.Offset(10, TitleBarHeight + 30);
                YAxisBounds.Width = 65;
                return YAxisBounds;
            }
        }

        private RectangleF MenuPanelBounds =>
            new RectangleF(InnerBounds.X, InnerBounds.Y, InnerBounds.Width, TitleBarHeight);
        protected override void Layout()
        {
            var bounds = Bounds;
            bounds.Width = Math.Max(bounds.Width, MinWidth);
            bounds.Height = Math.Max(bounds.Height, MinHeight);

            Bounds = new RectangleF(Pivot, bounds.Size);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button != MouseButtons.Left) return base.RespondToMouseDown(sender, e);

            if (_plotSelector.Contains(e.CanvasLocation))
            {
                _plotSelector.Clicked(sender, e);
                return base.RespondToMouseDown(sender, e);
            }

            foreach (var kpi in _kpiPlots)
                if (kpi.Contains(e.CanvasLocation))
                {
                    kpi.Clicked(sender, e);
                    return base.RespondToMouseDown(sender, e);
                }

            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            // show properties dialog if mouse is in Y axis box
            if (YAxisBounds.Contains(e.CanvasLocation) && AxisLimitPlots.Contains(_plotSelector._currentPlot.ToString()))
            {
                var propertiesDialog = new VisualizerPlotProperties();
                propertiesDialog.PlotParameters = Owner.PlotProperties;
                propertiesDialog.ShowDialog();

                Owner.ExpirePreview(true);
                Owner.ExpireSolution(true);
            }
            
            return GH_ObjectResponse.Release;
        }

        //this might be a really bad idea performance-wise?
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (YAxisBounds.Contains(e.CanvasLocation) && AxisLimitPlots.Contains(_plotSelector._currentPlot.ToString()))
            {
                InYAXisBounds = true;
                sender.Invalidate();
            }
            else
            {
                InYAXisBounds = false;
                sender.Invalidate();
            }
            return base.RespondToMouseMove(sender, e);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires && Owner.SourceCount > 0)
                RenderIncomingWires(canvas.Painter, Owner.Sources, Owner.WireDisplay);
            if (channel != GH_CanvasChannel.Objects)
                return;

            RenderCapsule(graphics);
            RenderBackground(graphics);
            RenderPlot(graphics);
            RenderTitleBar(graphics);
            RenderYAxisBox(graphics);
        }

        private void RenderBackground(Graphics graphics) => graphics.FillRectangle(new SolidBrush(Color.White), InnerBounds);

        private void RenderYAxisBox(Graphics graphics)
        {
            if (InYAXisBounds)
            {
                graphics.DrawRectangleF(new Pen(Color.Gray, 2), YAxisBounds);
            }
        }
        /// <summary>
        ///     Render the title bar at the top with the dropdown for selecting the plot and the
        ///     operational performance metrics.
        /// </summary>
        /// <param name="graphics"></param>
        private void RenderTitleBar(Graphics graphics)
        {
            _plotSelector.RenderMenuPanel(Owner.Results, graphics, MenuPanelBounds);

            // render the three operational performance plots
            var plotWidth = TitleBarHeight; // squares
            var bounds = new RectangleF(InnerBounds.Right - plotWidth, InnerBounds.Location.Y, plotWidth,
                TitleBarHeight);
            foreach (var kpi in _kpiPlots)
            {
                kpi.Normalized = _plotSelector.Normalized;
                kpi.Render(Owner.Results, graphics, bounds, _plotSelector.CurrentKpi == kpi.Kpi);
                bounds.Offset(-(plotWidth + Padding), 0);
            }

            
            bounds.Offset(-(plotWidth + Padding), 0);

        }

        public void RenderPlot(Graphics graphics)
        {
            _plotSelector.RenderCurrentPlot(Owner.Results, Owner.PlotProperties, graphics, PlotBounds);
        }

        private void RenderCapsule(Graphics graphics)
        {
            var capsule = Owner.RuntimeMessageLevel != GH_RuntimeMessageLevel.Error
                ? GH_Capsule.CreateCapsule(Bounds, GH_Palette.White, 5, 30)
                : GH_Capsule.CreateCapsule(Bounds, GH_Palette.Error, 5, 30);
            capsule.SetJaggedEdges(false, true);
            capsule.AddInputGrip(InputGrip);
            capsule.Render(graphics, Selected, Owner.Locked, true);
            capsule.Dispose();
        }

        public void NewData(ResultsPlotting results)
        {
            // this is where we would implement caching..
        }
    }
}