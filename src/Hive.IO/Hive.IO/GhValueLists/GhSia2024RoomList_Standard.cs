using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Util;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace Hive.IO.GhValueLists
{
    public class GHSia2024RoomList_Standard : GH_ValueList
    {
        public GHSia2024RoomList_Standard()
        {
            this.Name = "SIA 2024 Standard Rooms";
            this.NickName = "Sia2024RoomsStandard";
            this.Description = "A list of SIA 2024 rooms, standard building quality";
            this.Category = "[hive]";
            this.SubCategory = "Demand";
            Load();
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public struct SIA2024RoomListItem
        {
            public string description;
            public string Zeitkonstante;
            public string RaumlufttemperaturAuslegungKuehlungSommer;
            public string RaumlufttemperaturAuslegungKuehlungSommerAbsenktemperatur;
            public string RaumlufttemperaturAuslegungHeizenWinter;
            public string RaumlufttemperaturAuslegungHeizenWinterAbsenktemperatur;
            public string Nettogeschossflaeche;
            public string ThermischeGebaeudehuellflaeche;
            public string Glasanteil;
            public string UWertOpakeBauteile;
            public string UWertFenster;
            public string AbminderungsfaktorfuerFensterrahmen;
            public string WaermespeicherfaehigkeitdesRaumes;
            public string AussenluftVolumenstromproNGF;
            public string AussenluftVolumenstromdurchInfiltration;
            public string TemperaturAenderungsgradDerWaermerueckgewinnung;
            public string WaermeeintragsleistungPersonenBei24degCbzw70W;
            public string WaermeeintragsleistungDerRaumbeleuchtung;
            public string WaermeeintragsleistungDerGeraete;
            public string VollaststundenProJahrPersonen;
            public string JaehrlicheVollaststundenDerRaumbeleuchtung;
            public string JaehrlicheVollaststundenderGeraete;
            public string GesamtenergiedurchlassgradVerglasung;
            public string GesamtenergiedurchlassgradVerglasungUndSonnenschutz;
            public string KostenOpakeBauteile;
            public string KostenTransparenteBauteile;
            public string EmissionenOpakeBauteile;
            public string EmissionenTransparenteBauteile;
            public string StrahlungsleistungfuerBetaetigungSonnenschutz;
            public string JaehrlicherWaermebedarffuerWarmwasser;

        }

        private static List<SIA2024RoomListItem> roomList_; //JsonResource backing field

        public static string ResourceName = "Hive.IO.GhValueLists.201008_SIA2024_Raumdaten_Standardwert.json";

        List<SIA2024RoomListItem> roomList => JsonResource.ReadRecords(ResourceName, ref roomList_);

        private void Load()
        {
            this.ListItems.Clear();
            foreach (var item in roomList)
            {
                this.ListItems.Add(new GH_ValueListItem(item.description, item.description));
            }
        }

        public override Guid ComponentGuid => new Guid("8360f734-0ec9-4a71-8068-eb00ce15d96a");
    }
}
