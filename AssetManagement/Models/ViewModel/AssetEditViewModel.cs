using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetEditViewModel
    {
        public int AssetID { get; set; }

        public string AssetName { get; set; }

        public string AssetDescription { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public int? VendorId { get; set; }

        public string VendorName { get; set; }

        public int? DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int? LocationId { get; set; }

        public string LocationName { get; set; }

        public int? StatusId { get; set; }

        public int? CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public int? UsedBy { get; set; }

        public string UsedByName { get; set; }

        public decimal? UnitOfPrice { get; set; }

        public string AssetTag { get; set; }
    }
}