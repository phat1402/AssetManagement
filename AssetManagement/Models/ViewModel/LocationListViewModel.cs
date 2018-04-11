using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class LocationListViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? FloorNo { get; set; }
        public int? RoomNo { get; set; }
        public string BuildingName { get; set; }
    }
}