using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Util;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace Hive.IO.GhValueLists
{
    public class GHSia2024RoomList_Ideal : GH_ValueList
    {
        public GHSia2024RoomList_Ideal()
        {
            this.Name = "SSIA 2024 Ideal Room";
            this.NickName = "Sia2024RoomsIdeal";
            this.Description = "A list of SIA 2024 rooms, ideal building quality";
            this.Category = "[hive]";
            this.SubCategory = "Demand";
            Load();
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public struct SIA2024RoomIdealListItem
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

        private static List<SIA2024RoomIdealListItem> roomList_; //JsonResource backing field

        public static string ResourceName = "Hive.IO.GhValueLists.201008_SIA2024_Raumdaten_Zielwert.json";

        List<SIA2024RoomIdealListItem> roomList => JsonResource.ReadRecords(ResourceName, ref roomList_);

        private void Load()
        {
            this.ListItems.Clear();
            foreach (var item in roomList)
            {
                this.ListItems.Add(new GH_ValueListItem(item.description, item.description));
            }
        }

        public override Guid ComponentGuid => new Guid("772400f7-ee9a-4e53-b33d-bf0d676f4249");
    }
}
