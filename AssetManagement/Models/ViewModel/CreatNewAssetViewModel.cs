using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class CreatNewAssetViewModel
    {
        public string AssetName { get; set; }
        public int AssetSubCategoryId { get; set; }
        public int PurchaseAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int VendorId { get; set; }
        public string AssetDescription { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public decimal UnitOfPrice { get; set; }
    }
}