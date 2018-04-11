using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Account
{
    public class ChangePasswordViewModel
    {
        public int ID { get; set; }
        
        public string CurrentPass { get; set; }

        public string NewPass { get; set; }

        public string RetypePass { get; set; }
    }
}