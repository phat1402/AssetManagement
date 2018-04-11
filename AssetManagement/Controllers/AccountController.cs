using AssetManagement.Models;
using AssetManagement.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            if (ModelState.IsValid)
            {
                var user = Db.ApplicationUsers.FirstOrDefault(x => (x.Email == model.Email && x.Password == model.Password));
                if (user != null)
                {
                    var userRole = Db.UserRoles.FirstOrDefault(x => x.ID == user.RoleID).Label;
                    var profileData = new UserProfileSessionData
                    {
                        FullName = user.Firstname + " " + user.Lastname,
                        UserId = user.ID,
                        UserRole = userRole
                    };
                    this.Session["UserProfile"] = profileData;
                    return RedirectToAction("Index", "Home");
                }
                return null;
            }
            return View("~/Views/Account/Index.cshtml");
        }

        [HttpPost]
        public ActionResult Register(LoginViewModel model)
        {
            var user = new ApplicationUser
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Country = model.Country,
                CreatedDate = DateTime.Now,
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone,
            };
            Db.ApplicationUsers.Add(user);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}