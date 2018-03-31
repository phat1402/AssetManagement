using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetDisposalItem
    {
        public int ID { get; set; }

        public int AssetID { get; set; }

        public string AssetName { get; set; }

        public string AssetTag { get; set; }

        public string DiposedBy { get; set; }

        public DateTime DisposalDate { get; set; }

        public string Comment { get; set; }
    }
}