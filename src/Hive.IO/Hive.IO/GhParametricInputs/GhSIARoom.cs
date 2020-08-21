using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;


namespace Hive.IO.GhParametricInputs
{
    public class GhSIARoom : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhParametricInputSIARoom class.
        /// </summary>
        public GhSIARoom()
          : base("Parametric Input SIARoom Hive", "HiveParaInRoom",
              "Description",
              "[hive]", "IO")
        {
        }

        
        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("descriptiont", "description", "SIA 2024 room name. Must follow hive sia 2024 naming convention", GH_ParamAccess.item);
            pManager.AddIntegerParameter("roomConstant", "roomConstant", "Room constant ('Zeitkonstante'), in hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("coolingSetpoint", "coolingSetpoint", "Cooling setpoint 'Raumlufttemperatur Auslegung Kuehlung (Sommer)'. in deg Celsius", GH_ParamAccess.item);
            pManager.AddNumberParameter("heatingSetpoint", "heatingSetpoint", "Heating setpoint 'Raumlufttemperatur Auslegung Heizen (Winter)'. in deg Celsius", GH_ParamAccess.item);
            pManager.AddNumberParameter("floorArea", "floorArea", "Floor area 'Nettogeschossflaeche' in m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("envelopeArea", "envelopeArea", "Envelope Area 'Thermische Gebaeudehuellflaeche' in m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("glazingRatio", "glazingRatio", "Glazing ratio 'Glasanteil' [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("UValueOpaque", "UValueOpaque", "U-value opaque construction 'U-Wert opake Bauteile', in W/(m^2K)", GH_ParamAccess.item);
            pManager.AddNumberParameter("UValueTransparent", "UValueTransparent", "U-value transparent construction 'U-Wert Fenster', in W/^(m^2K)", GH_ParamAccess.item);
            pManager.AddNumberParameter("gValue", "gValue", "g-value of windows 'Gesamtenergiedurchlassgrad Verglasung' [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("windowFrameReduction", "windowFrameReduction", "Window frame reduction factor 'Abminderungsfaktor fuer Fensterrahmen', [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("airChangeRate", "airChangeRate", "Air change rate 'Aussenluft-Volumenstrom (pro NGF)', in m^3/(m^2h)", GH_ParamAccess.item);
            pManager.AddNumberParameter("infiltration", "infiltration", "Infiltration 'Aussenluft-Volumenstrom durch Infiltration', in m^3/(m^2h)", GH_ParamAccess.item);
            pManager.AddNumberParameter("heatRecovery", "heatRecovery", "Heat recovery 'Temperatur-Aenderungsgrad der Waermerueckgewinnung', [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("occupantLoads", "occupantLoads", "Internal loads from occupants 'Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)', in W/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("lightingLoads", "lightingLoads", "Internal loads form lighting 'Waermeeintragsleistung der Raumbeleuchtung', in W/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("equipmentLoads", "equipmentLoads", "Internal loads from equipment 'Waermeeintragsleistung der Geraete', in W/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("occupantYearlyHours", "occupantYearlyHours", "Yearly hours of full occupation 'Vollaststunden pro Jahr (Personen)', in hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("lightingYearlyHours", "lightingYearlyHours", "Yearly hours of full lighting usage 'Jaehrliche Vollaststunden der Raumbeleuchtung', in hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("equipmentYearlyHours", "equipmentYearlyHours", "Yearly hours of full equipment usage 'Jaehrliche Vollaststunden der Geraete', in hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("opaqueCost", "opaqueCost", "Cost for opaque construction 'Kosten opake Bauteil', in CHF/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("transparentCost", "transparentCost", "Cost for transparent construction 'Kosten transparente Bauteile', in CHF/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("opaqueEmissions", "opaqueEmissions", "Embodied green house gas emissions for opaque building construction 'Emissionen opake Bauteile', in kgCO2/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("transparentEmissions", "transparentEmissions", "Embodied green house gas emissions for transparent building construciton 'Emissionen transparente Bauteile', in kgCO2/m^2", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SIARoom", "SIARoom", "SIA Room as json, containing information about the building. Uses SIA 2024 naming conventions", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            SiaRoomProperties siaRoom = new SiaRoomProperties();

            DA.GetData(0, ref siaRoom.Description);
            DA.GetData(1, ref siaRoom.RoomConstant);
            DA.GetData(2, ref siaRoom.CoolingSetpoint);
            DA.GetData(3, ref siaRoom.HeatingSetpoint);
            DA.GetData(4, ref siaRoom.FloorArea);
            DA.GetData(5, ref siaRoom.EnvelopeArea);
            DA.GetData(6, ref siaRoom.GlazingRatio);
            DA.GetData(7, ref siaRoom.UValueOpaque);
            DA.GetData(8, ref siaRoom.UValueTransparent);
            DA.GetData(9, ref siaRoom.GValue);
            DA.GetData(10, ref siaRoom.WindowFrameReduction);
            DA.GetData(11, ref siaRoom.AirChangeRate);
            DA.GetData(12, ref siaRoom.Infiltration);
            DA.GetData(13, ref siaRoom.HeatRecovery);
            DA.GetData(14, ref siaRoom.OccupantLoads);
            DA.GetData(15, ref siaRoom.LightingLoads);
            DA.GetData(16, ref siaRoom.EquipmentLoads);
            DA.GetData(17, ref siaRoom.OccupantYearlyHours);
            DA.GetData(18, ref siaRoom.LightingYearlyHours);
            DA.GetData(19, ref siaRoom.EquipmentYearlyHours);
            DA.GetData(20, ref siaRoom.OpaqueCost);
            DA.GetData(21, ref siaRoom.TransparentCost);
            DA.GetData(22, ref siaRoom.OpaqueEmissions);
            DA.GetData(23, ref siaRoom.TransparentEmissions);

            var siaRoomDict = new Dictionary<string, object>();
            siaRoomDict.Add("description", siaRoom.Description);
            siaRoomDict.Add("Zeitkonstante", siaRoom.RoomConstant);
            siaRoomDict.Add("Raumlufttemperatur Auslegung Kuehlung (Sommer)", siaRoom.CoolingSetpoint);
            siaRoomDict.Add("Raumlufttemperatur Auslegung Heizen (Winter)", siaRoom.HeatingSetpoint);
            siaRoomDict.Add("Nettogeschossflaeche", siaRoom.FloorArea);
            siaRoomDict.Add("Thermische Gebaeudehuellflaeche", siaRoom.EnvelopeArea);
            siaRoomDict.Add("Glasanteil", siaRoom.GlazingRatio);
            siaRoomDict.Add("U-Wert opake Bauteile", siaRoom.UValueOpaque);
            siaRoomDict.Add("U-Wert Fenster", siaRoom.UValueTransparent);
            siaRoomDict.Add("Gesamtenergiedurchlassgrad Verglasung", siaRoom.GValue);
            siaRoomDict.Add("Abminderungsfaktor fuer Fensterrahmen", siaRoom.WindowFrameReduction);
            siaRoomDict.Add("Aussenluft-Volumenstrom (pro NGF)", siaRoom.AirChangeRate);
            siaRoomDict.Add("Aussenluft-Volumenstrom durch Infiltration", siaRoom.Infiltration);
            siaRoomDict.Add("Temperatur-Aenderungsgrad der Waermerueckgewinnung", siaRoom.HeatRecovery);
            siaRoomDict.Add("Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)", siaRoom.OccupantLoads);
            siaRoomDict.Add("Waermeeintragsleistung der Raumbeleuchtung", siaRoom.LightingLoads);
            siaRoomDict.Add("Waermeeintragsleistung der Geraete", siaRoom.EquipmentLoads);
            siaRoomDict.Add("Vollaststunden pro Jahr (Personen)", siaRoom.OccupantYearlyHours);
            siaRoomDict.Add("Jaehrliche Vollaststunden der Raumbeleuchtung", siaRoom.LightingYearlyHours);
            siaRoomDict.Add("Jaehrliche Vollaststunden der Geraete", siaRoom.EquipmentYearlyHours);
            siaRoomDict.Add("Kosten opake Bauteile", siaRoom.OpaqueCost);
            siaRoomDict.Add("Kosten transparente Bauteile", siaRoom.TransparentCost);
            siaRoomDict.Add("Emissionen opake Bauteile", siaRoom.OpaqueEmissions);
            siaRoomDict.Add("Emissionen transparente Bauteile", siaRoom.TransparentEmissions);

            string siaRoomJson = JsonConvert.SerializeObject(siaRoomDict);
            DA.SetData(0, siaRoomJson);
        }

        private class SiaRoomProperties
        {
            public string Description;
            public int RoomConstant;
            public double CoolingSetpoint;
            public double HeatingSetpoint;
            public double FloorArea;
            public double EnvelopeArea;
            public double GlazingRatio;
            public double UValueOpaque;
            public double UValueTransparent;
            public double GValue;
            public double WindowFrameReduction;
            public double AirChangeRate;
            public double Infiltration;
            public double HeatRecovery;
            public double OccupantLoads;
            public double LightingLoads;
            public double EquipmentLoads;
            public double OccupantYearlyHours;
            public double LightingYearlyHours;
            public double EquipmentYearlyHours;
            public double OpaqueCost;
            public double TransparentCost;
            public double OpaqueEmissions;
            public double TransparentEmissions;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5a628fc1-283e-4549-a94c-09fa7f232e97"); }
        }
    }
}