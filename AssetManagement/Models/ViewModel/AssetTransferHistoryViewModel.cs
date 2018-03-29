using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetTransferHistoryViewModel
    {
        public int ID { get; set; }
        public string AssetTag { get; set; }
        public string AssetName { get; set; }
        public DateTime TransferDate { get; set; }
        public string FromEmployee { get; set; }
        public string ToEmployee { get; set; }
        public string Note { get; set; }
    }
}