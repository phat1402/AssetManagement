using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetCheckInViewModel
    {
        public int AssetID { get; set; }
        public DateTime? CheckInDate { get; set; }
        public int StaffID { get; set; }
        public string Note { get; set; }
        public IEnumerable<LastAssetCheckInViewModel> LastCheckIn { get; set; }
    }
}

