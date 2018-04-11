using AssetManagement.Models;
using AssetManagement.Models.Account;
using AssetManagement.Models.Error;
using AssetManagement.Models.Setting;
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
                var error = new ErrorViewModel
                {
                    ErrorTitle = "Login Failed",
                    ErrorMessage = "You entered wrong password or email"
                };                  
                return View("~/Views/Error/ErrorPage.cshtml", error);
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

        public ActionResult ViewProfile(int userId)
        {
            var userInfor = Db.ApplicationUsers.Where(x => x.ID == userId)
                                               .Select(x => new UserProfileViewModel
                                               {
                                                   ID = x.ID,
                                                   Country = x.Country,
                                                   CreatedDate = x.CreatedDate.Value,
                                                   Email = x.Email,
                                                   Firstname = x.Firstname,
                                                   Lastname = x.Lastname,
                                                   Phone = x.Phone,
                                                   UpdatedDate = x.UpdatedDate.HasValue ? x.UpdatedDate.Value : default(DateTime)
                                               }).FirstOrDefault();
            var userList = Db.ApplicationUsers.Select(x => new SystemUserViewModel
            {
                ID = x.ID,
                Fullname = x.Firstname + " " + x.Lastname,
                Email = x.Email,
                CreatedDate = x.CreatedDate.Value,
                StatusId = x.StatusId.Value
            }).ToList();

            var setting = Db.CompanySettings.Select(x => new CompanySettingViewModel {
                CompanyDescription = x.Description,
                CompanyEmail = x.Email,
                LoginLink = x.LogoLink,
                CompanyPhone = x.Phone,
                CompanyID = x.ID
            }).FirstOrDefault();

            userInfor.UserList = userList;
            userInfor.Setting = setting;
            return View("~/Views/Account/UserProfile.cshtml", userInfor);
        }

        [HttpPost]
        public JsonResult UpdateUserInfor(UserProfileViewModel model)
        {
            try
            {
                var user = Db.ApplicationUsers.Find(model.ID);
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.Phone = model.Phone;
                user.Country = model.Country;
                if( Db.SaveChanges() > 0)
                {
                    var profileData = new UserProfileSessionData
                    {
                        FullName = user.Firstname + " " + user.Lastname,
                        UserId = user.ID,
                        UserRole = user.UserRole.Label
                    };
                    this.Session["UserProfile"] = profileData;
                    return Json(new { success = true, message = "Update user information successfully! " });
                }
                return Json(new { success = false, message = "Oops! Something went wrong" });
            }
            catch(Exception e)
            {
                return Json(e.Message);
            }
        }

        public JsonResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var user = Db.ApplicationUsers.Find(model.ID);
                if (user.Password != model.CurrentPass)
                {
                    return Json("Oops! The current password is not correct !");
                }
                if (model.NewPass != model.RetypePass)
                {
                    return Json("Your new password and retype passoword does not match!");
                }
                user.Password = model.NewPass;
                if (Db.SaveChanges() > 0)
                {
                    return Json("You changed password successfully");
                }
                else
                {

                    return Json("Something is wrong! Can not update new password!");
                }
            }
            catch(Exception e)
            {
                return Json(e.Message);
            }
        }
    }
}