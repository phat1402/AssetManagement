using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class EmployeeListViewModel
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}