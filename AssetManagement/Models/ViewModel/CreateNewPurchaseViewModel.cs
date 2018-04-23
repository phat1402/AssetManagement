using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class CreateNewPurchaseViewModel
    {
        public string AssetName { get; set; }
        public int AssetSubCategoryId { get; set; }
        public int PurchaseAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int VendorId { get; set; }
        public decimal UnitOfPrice { get; set; }
        public int StoreId { get; set; }
        public int CreatedBy { get; set; }
    }
}