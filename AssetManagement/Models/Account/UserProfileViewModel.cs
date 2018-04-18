using AssetManagement.Models.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Account
{
    public class UserProfileViewModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public ChangePasswordViewModel PasswordTab { get; set; }

        public IEnumerable<SystemUserViewModel> UserList { get; set; }
    }
}