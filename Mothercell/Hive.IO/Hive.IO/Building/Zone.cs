using System;
using System.Collections.Generic;
using System.Linq;

using rg = Rhino.Geometry;
using Hive.IO.BuildingComponents;


namespace Hive.IO
{

    /// <summary>
    /// Thermal Zone.
    /// Geometry must be (1) Brep, (2) closed, (3) convex, and (4) not contain any curves, i.e. lines only.
    /// </summary>
    public class Zone
    {
        #region Main Properties
        /// <summary>
        /// The actual zone geometry, as rhino Brep
        /// </summary>
        public rg.Brep ZoneGeometry { get; private set; }
        /// <summary>
        /// Unique index, used to identify the zone when it is part of a Building object
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Zone volume in [m^3]
        /// </summary>
        public double Volume { get; private set; }

        /// <summary>
        /// Tolerance for geometric operations. Get from RhinoDoc.ModelAbsoluteTolerance?
        /// </summary>
        public double Tolerance { get; private set; }
        /// <summary>
        /// fix the horizon to one year, hourly
        /// </summary>
        private const int _horizon = 8760;
        #endregion


        #region Loads And Schedules
        /// <summary>
        /// Zone name, e.g. 'Kitchen01'
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Internal loads.
        /// Values indicate full / maximal value.
        /// Unit in W/sqm
        /// </summary>
        public struct StructInternalLoads
        {
            public double Occupants;
            public double Devices;
            public double Lighting;
        }
        /// <summary>
        /// Internal loads structure. Values in W/m2
        /// </summary>
        public StructInternalLoads InternalLoads;

        /// <summary>
        /// Time-resolved schedules, values are [0, 1].
        /// Value of 1 indicates full load as defined in this.InternalLoads
        /// </summary>
        public struct StructSchedules
        {
            public double[] Occupants;
            public double[] Devices;
            public double[] Lighting;
        }
        /// <summary>
        /// Schedules that define annual hourly internal loads schedules
        /// </summary>
        public StructSchedules Schedule;
        #endregion


        #region Building Components
        /// <summary>
        /// Wall components of this zone. Cannot be empty.
        /// </summary>
        public Wall[] Walls { get; private set; }
        /// <summary>
        /// Ceiling components of this zone. Cannot be empty.
        /// </summary>
        public Ceiling[] Ceilings { get; private set; }
        /// <summary>
        /// Floor components of this zone. Cannot be empty. A void would also be a floor, but with material property 'air' or something
        /// </summary>
        public Floor[] Floors { get; private set; }
        /// <summary>
        /// Apertures of this zone, e.g. windows, skylights, doors, ventilation openings, etc.. Can be empty.
        /// </summary>
        public Opening[] Openings { get; private set; }
        /// <summary>
        /// Roof components of this zone. Can be empty.
        /// </summary>
        public Roof[] Roofs { get; private set; }

        /// <summary>
        /// Shading devices
        /// </summary>
        public Shading[] ShadingDevices { get; private set; }

        #endregion


        #region Energy Demand
        /// <summary>
        /// in kWh per month
        /// </summary>
        public double[] HeatingLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per month
        /// </summary>
        public double[] DHWLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per month
        /// </summary>
        public double[] CoolingLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per month
        /// </summary>
        public double[] ElectricityLoadsMonthly { get; private set; }
        #endregion


        #region Error handling
        /// <summary>
        /// For simplicity of thermal calculations, avoid curves etc., only accept linear floorplans and geometries
        /// </summary>
        public bool IsLinear { get; private set; }
        /// <summary>
        /// For simplicity of thermal calculations, only accept convex zones
        /// </summary>
        public bool IsConvex { get; private set; }
        /// <summary>
        /// Zone geometry must be a closed Brep, since it defines a thermal space
        /// </summary>
        public bool IsClosed { get; private set; }
        /// <summary>
        /// Check planarity of all surfaces. Must be for simplicity
        /// </summary>
        public bool IsPlanar { get; private set; }
        /// <summary>
        /// Main bool, if this is false, then no thermal simulations can be done
        /// </summary>
        public bool IsValid { get; private set; }
        /// <summary>
        /// Stricter validity check for EnergyPlus. Can still perform SIA and RC simulations
        /// </summary>
        public bool IsValidEPlus { get; private set; }
        /// <summary>
        /// Checking whether window surfaces (if any exist) are lying on the zone geometry. 
        /// Window surfaces associated to a zone cannot just lie somewhere else.
        /// </summary>
        public bool IsWindowsOnZone { get; private set; }
        /// <summary>
        /// Windows self intersection
        /// </summary>
        public bool IsWindowsNoSelfIntersect { get; private set; }
        public bool IsFloorInZone { get; private set; }
        public string ErrorText { get; private set; }
        #endregion


        #region Constructor
        /// <summary>
        /// Create an instance of a Zone object
        /// </summary>
        /// <param name="zone_geometry">Brep geometry. Must be closed, linear and convex.</param>
        /// <param name="index">Unique identifier</param>
        /// <param name="name">Zone name, e.g. kitchen 1</param>
        public Zone(rg.Brep zone_geometry, int index, double tolerance, string name, rg.BrepFace[] openingSrfs = null, rg.BrepFace[] floorSrfs = null, rg.BrepFace[] shadingSrfs = null)
        {
            this.ZoneGeometry = zone_geometry;
            this.Index = index;
            this.Tolerance = tolerance;

            // only IsClosed needs to strictly guaranteed in all cases
            this.IsClosed = false; 
            this.IsConvex = false;
            this.IsPlanar = false;
            this.IsWindowsOnZone = true; // zone might have no windows. so default is true
            this.IsWindowsNoSelfIntersect = true;
            this.IsFloorInZone = true;

            this.IsClosed = CheckClosedness(this.ZoneGeometry);
            if (this.IsClosed)
            {
                this.IsLinear = CheckLinearity(this.ZoneGeometry);
                this.IsPlanar = CheckPlanarity(this.ZoneGeometry);
                this.IsConvex = CheckConvexity(this.ZoneGeometry, this.Tolerance);
            }

            if (openingSrfs.Length > 0)
            {
                this.IsWindowsOnZone = CheckWindowsOnZone(this.ZoneGeometry, openingSrfs, this.Tolerance);
                this.IsWindowsNoSelfIntersect = CheckWindowsSelfIntersect(openingSrfs, this.Tolerance);
            }
            this.IsValidEPlus = CheckValidity(this.IsClosed, this.IsConvex, this.IsLinear, this.IsPlanar, this.IsWindowsOnZone, this.IsWindowsNoSelfIntersect);
            this.IsValid = (this.IsClosed && this.IsWindowsOnZone && this.IsWindowsNoSelfIntersect) ? true : false;
            this.ErrorText = String.Format("IsLinear: {0} \n " + "IsConvex: {1} \n " + "IsClosed: {2} \n " + "IsPlanar: {3} \n "
    + "IsWindowsOnZone: {4} \n " + "IsWindowsSelfIntersect: {5} \n" + "IsFloorInZone: {6}",
    this.IsLinear, this.IsConvex, this.IsClosed, this.IsPlanar, this.IsWindowsOnZone, this.IsWindowsNoSelfIntersect, this.IsFloorInZone);

            if (this.IsValid)
            {
                Tuple<Wall[], Ceiling[], Roof[], Floor[], Opening[], Shading[]> tuple = IdentifyComponents(zone_geometry, openingSrfs, shadingSrfs);
                this.Walls = tuple.Item1;
                this.Ceilings = tuple.Item2;
                this.Roofs = tuple.Item3;
                this.Floors = tuple.Item4;
                this.Openings = tuple.Item5;
                this.ShadingDevices = tuple.Item6;

                this.Volume = zone_geometry.GetVolume();
            }
            else
            {
                return;
            }


            if (floorSrfs.Length > 0)
            {
                var floorList = new List<rg.BrepFace>();
                foreach(var floor in floorSrfs)
                {
                    if (CheckFloorInZone(zone_geometry, floor))
                        floorList.Add(floor);
                    else
                        this.IsFloorInZone = false;
                }
                foreach (var floorOld in this.Floors)
                    floorList.Add(floorOld.BrepGeometry.Faces[0]);
                this.Floors = new Floor[floorList.Count];
                for (int i = 0; i < floorList.Count; i++)
                    this.Floors[i] = new Floor(floorList[i]);
            }

            // define standard building physical properties upon inizialization. 
            // Can be changed later via Windows Form
            this.Name = name;
            this.InternalLoads.Occupants = 16.0;
            this.InternalLoads.Lighting = 4.0;
            this.InternalLoads.Devices = 3.0;
            this.Schedule.Occupants = new double[_horizon];
            this.Schedule.Lighting = new double[_horizon];
            this.Schedule.Devices = new double[_horizon];
            // windows form with interface to change schedules for workdays and weekends / holidays?
            for (int i = 0; i < _horizon; i++)
            {
                this.Schedule.Occupants[i] = 1.0;
                this.Schedule.Lighting[i] = 1.0;
                this.Schedule.Devices[i] = 1.0;
            }




        }
        #endregion


        #region Setters

        /// <summary>
        /// Setting monthly energy demands of this zone. Loads have to be computed externally, e.g. with Hive.Core SIA380
        /// </summary>
        /// <param name="heatingLoads"></param>
        /// <param name="dhwLoads"></param>
        /// <param name="coolingLoads"></param>
        /// <param name="electricityLoads"></param>
        public void SetEnergyDemandsMonthly(double[] heatingLoads, double[] dhwLoads, double[] coolingLoads, double[] electricityLoads)
        {
            const int months = 12;
            this.HeatingLoadsMonthly = new double[months];
            this.DHWLoadsMonthly = new double[months];
            this.CoolingLoadsMonthly = new double[months];
            this.ElectricityLoadsMonthly = new double[months];

            heatingLoads.CopyTo(this.HeatingLoadsMonthly, 0);
            dhwLoads.CopyTo(this.DHWLoadsMonthly, 0);
            coolingLoads.CopyTo(this.CoolingLoadsMonthly, 0);
            electricityLoads.CopyTo(this.ElectricityLoadsMonthly, 0);
        }

        #endregion



        #region internalMethods

        private static bool CheckFloorInZone(rg.Brep zone, rg.BrepFace floor)
        {
            //check if all points of middle floor are inside brep or on face.
            //but not all points are on face, otherwise it would be redundant(same as wall or base floor)

            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var pts = new List<rg.Point3d>();

            foreach (rg.BrepVertex vertex in floor.Brep.Vertices)
            {
                rg.Point3d pt = vertex.Location;
                if (!zone.IsPointInside(pt, tol, false))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Check the linearity of the brep zone geometry. No curves allowed for simplicity.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True if all is linear</returns>
        private static bool CheckLinearity(rg.Brep brep)
        {
            bool isLinear = true;
            foreach (rg.BrepEdge edge in brep.Edges)
            {
                if (edge.IsLinear() == false)
                {
                    isLinear = false;
                    break;
                }
            }
            return isLinear;
        }

        /// <summary>
        /// Check the convexity of the zone. Hive only allows convex spaces, for simplicity.
        /// </summary>
        /// <param name="brep">geometry that is checked for convexity</param>
        /// <param name="tolerance">model tolerance, for intersection checks</param>
        /// <returns>True, if convex</returns>
        private static bool CheckConvexity(rg.Brep brep, double tolerance)
        {
            int vertexCount = brep.Vertices.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                rg.BrepVertex vert1 = brep.Vertices[i];
                for (int u = 0; u < vertexCount; u++)
                {
                    if (i == u) continue;

                    rg.BrepVertex vert2 = brep.Vertices[u];
                    rg.LineCurve line = new rg.LineCurve(vert1.Location, vert2.Location);
                    rg.Curve[] overlap_curves;
                    rg.Point3d[] inter_points;
                    if (rg.Intersect.Intersection.CurveBrep(line, brep, tolerance, out overlap_curves, out inter_points))
                    {
                        if (overlap_curves.Length > 0 || inter_points.Length > 0)
                        {
                            if (inter_points.Length > 2)
                                return false;
                            else if (inter_points.Length == 2)
                            {
                                //check if middle point is within the brep or not. if not, its convex
                                rg.Point3d middlepoint = (inter_points[0] + inter_points[1]) / 2.0;
                                if (!brep.IsPointInside(middlepoint, tolerance, false))
                                {
                                    return false;
                                }
                            } //do i need a case with inter_points == 1?
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check the closedness of the brep. No open breps allowed, since a the brep is used to define a thermal space.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True, if it is closed</returns>
        private static bool CheckClosedness(rg.Brep brep)
        {
            return brep.IsSolid;
        }

        /// <summary>
        /// Check for planarity of surfaces
        /// </summary>
        /// <param name="brep"></param>
        /// <returns></returns>
        private static bool CheckPlanarity(rg.Brep brep)
        {
            rg.Collections.BrepSurfaceList srfs = brep.Surfaces;
            foreach (rg.Surface srf in srfs)
            {
                if (!srf.IsPlanar()) return false;
            }
            return true;
        }


        // !!!!!!!!!!! TO DO
        // ADAPT to work for non-planar and non-linear geometry

        /// <summary>
        /// Check whether window surfaces lie on the zone geometry.
        /// </summary>
        /// <param name="brep"></param>
        /// <param name="windows"></param>
        /// <returns></returns>
        private static bool CheckWindowsOnZone(rg.Brep brep, rg.BrepFace[] windows, double tolerance)
        {
            int roundingDecimals = tolerance.ToString().Split('.')[1].Length;

            // check for Windows on Zone
            bool[] equalAreas = new bool[windows.Length];
            for (int i = 0; i < windows.Length; i++)
            {
                rg.BrepFace srf = windows[i];
                rg.Brep srfbrep = rg.Brep.CreateTrimmedSurface(srf, srf.UnderlyingSurface(), tolerance);
                rg.Curve[] intersectionCrvs;
                rg.Point3d[] intersectionPts;
                //rg.Intersect.Intersection.BrepSurface(brep, srf, tolerance, out intersectionCrvs, out intersectionPts);
                rg.Intersect.Intersection.BrepBrep(brep, srfbrep, tolerance, out intersectionCrvs, out intersectionPts);
                rg.Curve curve = intersectionCrvs[0];
                if (intersectionCrvs.Length > 1)
                {
                    rg.Curve [] crv = rg.Curve.JoinCurves(intersectionCrvs);
                    if (crv.Length > 1 && !crv[0].IsClosed)
                        return false;
                    else 
                        curve = crv[0];
                }
                double curveArea = rg.AreaMassProperties.Compute(curve).Area;
                double srfArea = rg.AreaMassProperties.Compute(srf).Area;
                if (Math.Round(curveArea, roundingDecimals) != Math.Round(srfArea, roundingDecimals))
                    return false;
                else
                    equalAreas[i] = true;
            }
            foreach (bool equalArea in equalAreas)
                if (!equalArea)
                    return false;

            return true;
        }


        // !!!!!!!!!!! TO DO
        // ADAPT to work for non-planar and non-linear geometry

        /// <summary>
        /// Check for self-intersection of windows
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private static bool CheckWindowsSelfIntersect(rg.Surface[] windows, double tolerance)
        {
            for (int i = 0; i < windows.Length - 1; i++)
            {
                rg.Brep w1 = windows[i].ToBrep();
                for (int u = (i + 1); u < windows.Length; u++)
                {
                    rg.Curve[] intersectionCrvs;
                    rg.Point3d[] intersectionPts;
                    //misses overlaps ?!
                    //rg.Intersect.Intersection.SurfaceSurface(windows[i], windows[u], tolerance, out intersectionCrvs, out intersectionPts);
                    rg.Intersect.Intersection.BrepSurface(w1, windows[u], tolerance, out intersectionCrvs, out intersectionPts);
                    if (intersectionPts.Length > 0)
                        return false;
                    if (intersectionCrvs.Length > 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if all conditions are fulffilled.
        /// </summary>
        /// <returns>True, if zone geometry is valid</returns>
        private static bool CheckValidity(bool closed, bool convex, bool linear, bool planar, bool windowsOnZone, bool windowsSelfIntersect)
        {
            if (convex && closed && linear && planar && windowsOnZone && windowsSelfIntersect)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Identifies and initializes building components from input geometries
        /// </summary>
        /// <param name="zone_geometry"></param>
        /// <param name="openings_geometry"></param>
        /// <param name="shading_geometry"></param>
        /// <returns></returns>
        private static Tuple<Wall[], Ceiling[], Roof[], Floor[], Opening[], Shading[]>
            IdentifyComponents(rg.Brep zone_geometry, rg.BrepFace[] openings_geometry, rg.BrepFace[] shading_geometry)
        {
            Opening[] openings = new Opening[0];
            Shading[] shadings = new Shading[0];
            if (openings_geometry != null && openings_geometry.Length > 0)
            {
                openings = new Opening[openings_geometry.Length];
                for (int i = 0; i < openings.Length; i++)
                {
                    openings[i] = new Opening(openings_geometry[i]);
                }
            }

            if (shading_geometry != null && shading_geometry.Length > 0)
            {
                shadings = new Shading[shading_geometry.Length];
                for (int i = 0; i < shading_geometry.Length; i++)
                {
                    shadings[i] = new Shading(shading_geometry[i]);
                }
            }

            List<int> wall_indices = new List<int>();
            List<int> ceiling_indices = new List<int>();
            List<int> roof_indices = new List<int>();
            List<int> floor_indices = new List<int>();

            for (int i = 0; i < zone_geometry.Faces.Count(); i++)
            {
                rg.BrepFace srf = zone_geometry.Faces[i];
                srf.ClosestPoint(rg.AreaMassProperties.Compute(srf).Centroid, out double u, out double v);
                rg.Vector3d normal = srf.NormalAt(u, v); // for some reason, the bottom surface also has postivie normal here?!... using wrong point at line above?
                double angle = rg.Vector3d.VectorAngle(normal, new rg.Vector3d(0, 0, 1)) * 180 / Math.PI;

                // Floor: flat surface with  normal pointing downwards. 
                //  but careful, it could also be an overhanging wall. so floor is that surface with the lowest corner point
                //  lets say, floor MUST be flat
                // Ceiling: Same, but there must be an adjacent zone surface, such that this surface is internal. Hive 0.2
                if (normal.Z == -1.0)
                {
                    floor_indices.Add(i);
                }
                else if (angle < 45.0)                  // Roof: surface angle < 45? 
                {
                    roof_indices.Add(i);
                }
                else                                    // Wall: surface angle >= 45?
                {
                    wall_indices.Add(i);
                }
            }
            Wall[] walls = new Wall[wall_indices.Count()];
            Ceiling[] ceilings = new Ceiling[ceiling_indices.Count()];
            Roof[] roofs = new Roof[roof_indices.Count()];
            Floor[] floors = new Floor[floor_indices.Count()];

            for (int i = 0; i < walls.Length; i++)
                walls[i] = new Wall(zone_geometry.Faces[wall_indices[i]]);
            for (int i = 0; i < ceilings.Length; i++)
                ceilings[i] = new Ceiling(zone_geometry.Faces[ceiling_indices[i]]);
            for (int i = 0; i < roofs.Length; i++)
                roofs[i] = new Roof(zone_geometry.Faces[roof_indices[i]]);
            for (int i = 0; i < floors.Length; i++)
                floors[i] = new Floor(zone_geometry.Faces[floor_indices[i]]);


            return new Tuple<Wall[], Ceiling[], Roof[], Floor[], Opening[], Shading[]>(walls, ceilings, roofs, floors, openings, shadings);
        }
        #endregion
    }

}
