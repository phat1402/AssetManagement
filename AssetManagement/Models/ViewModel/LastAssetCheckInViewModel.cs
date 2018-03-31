using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class LastAssetCheckInViewModel
    {
        public int ID { get; set; }
        public int AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetTag { get; set; }
        public DateTime CheckInDate { get; set; }
        public string AssignToName { get; set; }
        public string Note { get; set; }
    }
}