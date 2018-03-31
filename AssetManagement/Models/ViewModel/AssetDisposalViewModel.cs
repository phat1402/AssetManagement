using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetDisposalViewModel
    {
        public int AssetID { get; set; }

        public DateTime? DisposalDate { get; set; }

        public string Comment { get; set; }

        public int DisposedBy { get; set; }

        public IEnumerable<AssetDisposalItem> DisposalHistory { get; set; }

    }
}