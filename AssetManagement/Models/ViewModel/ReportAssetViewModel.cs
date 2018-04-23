using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class ReportAssetViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string SubCategory { get; set; }
        public string Category { get; set; }
        public int StatusID { get; set; }
        public string UsedByFullname { get; set; }
        public string BuildingName { get; set; }
        public string DepartmentName { get; set; }
        public string StoreName { get; set; }
    }
}