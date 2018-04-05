using AssetManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AssetManagement.DAL
{
    public class AssetManagementContext: DbContext
    {
        public AssetManagementContext() : base("AssetManagementEntities"){}
        DbSet<Asset> Assets { get; set; }
        DbSet<Asset_Disposal> AssetDisposals { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Staff> Staffs { get; set; }
        DbSet<SubCategory> SubCategories { get; set; }
        DbSet<Vendor> Vendors { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
    }
}