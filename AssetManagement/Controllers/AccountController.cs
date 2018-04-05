using AssetManagement.Models;
using AssetManagement.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssetManagement.Controllers
{
    public class AccountController : Controller
    {
        AssetManagementEntities Db = new AssetManagementEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var user = Db.ApplicationUsers.FirstOrDefault(x => (x.Email == model.Email && x.Password == model.Password));
            if( user != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return null;
        }
    }
}