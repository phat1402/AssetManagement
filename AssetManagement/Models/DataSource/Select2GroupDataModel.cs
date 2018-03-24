using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.DataSource
{
    public class Select2GroupDataModel
    {
        public string text { get; set; }
        public List<Select2DataModel> children { get; set; }
    }
}