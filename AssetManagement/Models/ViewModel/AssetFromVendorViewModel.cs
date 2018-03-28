using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetFromVendorViewModel
    {
        public string VendorName { get; set; }
        public IEnumerable<AssetListViewModel> AssetList { get; set; }
    }
}