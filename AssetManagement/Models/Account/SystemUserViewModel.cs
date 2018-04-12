using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Account
{
    public class SystemUserViewModel
    {
        public int ID { get; set; }

        public string Fullname { get; set; }

        public string Email { get; set; }

        public int StatusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Role { get; set; }
    }
}