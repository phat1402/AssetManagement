using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Dashboard
{
    public class DataList
    {
        public IEnumerable<CategoryDetail> CategoryDataList { get; set; }
        public IEnumerable<VendorDetail> VendorDataList { get; set; }
        public IEnumerable<DepartmentDetail> DepartmentDataList { get; set; }
    }
}