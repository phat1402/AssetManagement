using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssetManagement.Models.Account
{
    public class LoginRegisterViewModel
    {
        [Required(ErrorMessage = "The email address is required") ]
        [EmailAddress(ErrorMessage = "Invalid Email Address") ]
        public string Email { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        public string RegisterEmail { get; set; }

        [Required(ErrorMessage = "The password is required")]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string RegisterPassword { get; set; }

        [Required(ErrorMessage = "The retype password is required")]
        public string RetypePassword { get; set; }

        public string Phone { get; set; }

        public string Country { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string Lastname { get; set; }

    }
}