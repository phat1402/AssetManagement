using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetDetailViewModel
    {
        public int AssetID { get; set; }

        public string AssetName { get; set; }

        public string AssetDescription { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string SubCategory { get; set; }

        public string Category { get; set; }

        public string Vendor { get; set; }

        public string Department { get; set; }

        public string Location { get; set; }

        public int? StatusId { get; set; }

        public string CreatedBy { get; set; }

        public string UsedBy { get; set; }

        public decimal? UnitOfPrice { get; set; }

        public string AssetTag { get;set;}

        public IEnumerable<AssetTransferHistoryViewModel> TransferHistory { get; set; }

        public IEnumerable<LastAssetCheckInViewModel> CheckInHistory { get; set; }

        public IEnumerable<LastAssetCheckOutViewModel> CheckOutHistory { get; set; }
    }
}