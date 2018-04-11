using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Setting
{
    public class CompanySettingViewModel
    {
        public int CompanyID { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string LoginLink { get; set; }
    }
}