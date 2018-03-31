using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetCheckOutViewModel
    {
        public int AssetID { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int StaffID { get; set; }
        public string Note { get; set; }
        public IEnumerable<LastAssetCheckOutViewModel> LastCheckOut { get; set; }
    }
}