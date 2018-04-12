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

        public RegisterViewModel Register { get; set; }
    }
}