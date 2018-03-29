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
        public DateTime? TransferDate { get; set; }
        public int FromEmployeeId { get; set; }
        public int ToEmployeeId { get; set; }
        public string Note { get; set; }
        public IEnumerable<AssetTransferHistoryViewModel> TransferHistoryList { get; set; }
    }
}