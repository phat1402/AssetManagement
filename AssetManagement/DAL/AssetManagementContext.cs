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
        public AssetManagementContext() : base("AssetManagementEntities")
        {

        }
        DbSet<Asset> Assets { get; set; }
        DbSet<Asset_Disposal> AssetDisposals { get; set; }
        
    }
}