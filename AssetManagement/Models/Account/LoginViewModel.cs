using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The email address is required") ]
        [EmailAddress(ErrorMessage = "Invalid Email Address") ]
        public string Email { get; set; }

        public string Password { get; set; }

        public string RegisterEmail { get; set; }

        public string RegisterPassword { get; set; }

        public string RetypePassword { get; set; }

        public string Phone { get; set; }

        public string Country { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }
    }
}