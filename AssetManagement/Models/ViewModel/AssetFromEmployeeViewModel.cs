using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class AssetFromEmployeeViewModel
    {
        public string EmployeeFullName { get; set; }
        public IEnumerable<AssetListViewModel> AssetList { get; set; }
    }
}