using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class PrintLabelViewModel
    {
        public int AssetID { get; set; }

        public string AssetName { get; set; }

        public string AssetTag { get; set; }
    }
}