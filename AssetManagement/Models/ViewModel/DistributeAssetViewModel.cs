using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class DistributeAssetViewModel
    {
        public int AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetTag { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int AssignToId { get; set; }
    }
}