using AssetManagement.Models;
using AssetManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssetManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AssetManagementEntities Db = new AssetManagementEntities();
            DashboardViewModel model = new DashboardViewModel();
            int numberOfAsset = Db.Assets.Count();

            return View();
        }

    }
}