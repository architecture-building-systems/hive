using Grasshopper.Kernel.Special;
using System;
using Hive.IO.Util;
using Hive.IO.Building;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace Hive.IO.GhValueLists
{
    public class GHSia2024RoomList_Standard : GH_ValueList
    {
        public GHSia2024RoomList_Standard()
        {
            this.Name = "SIA 2024 Rooms";
            this.NickName = "Sia2024Rooms";
            this.Description = "A list of SIA 2024 rooms";
            this.Category = "[hive]";
            this.SubCategory = "Demand";
            Load();
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public struct SIA2024RoomListItem
        {
            public string description;

        }

        private static List<SIA2024RoomListItem> roomList_; //JsonResource backing field

        public static string ResourceName = "Hive.IO.GhValueLists.SIA2024_rooms_data.json";

        List<SIA2024RoomListItem> roomList =>  JsonResource.ReadRecords(ResourceName, ref roomList_);

        private void Load()
        {
            this.ListItems.Clear();
            foreach (var item in roomList)
            {
                this.ListItems.Add(new GH_ValueListItem(item.description, String.Format("\"{0}\"", item.description)));
            }
        }

        public override Guid ComponentGuid => new Guid("8360f734-0ec9-4a71-8068-eb00ce15d96a");
    }
}
