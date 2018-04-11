using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Account
{
    [Serializable]
    public class UserProfileSessionData
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string UserRole { get; set; }
    }
}