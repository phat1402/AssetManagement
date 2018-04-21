using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetTransferViewModel
    {
        public int AssetID { get; set; }

        [Required(ErrorMessage = "The transfer date is required")]
        public DateTime? TransferDate { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int FromEmployeeId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int ToEmployeeId { get; set; }

        public string Note { get; set; }

        public IEnumerable<AssetTransferHistoryViewModel> TransferHistoryList { get; set; }
    }
}