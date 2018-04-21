using AssetManagement.Models;
using AssetManagement.Models.Account;
using AssetManagement.Models.Common;
using AssetManagement.Models.Error;
using AssetManagement.Models.Setting;
using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionResult Login(LoginRegisterViewModel model)
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
        public ActionResult Register(LoginRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(Db.ApplicationUsers.Any(x => x.Email == model.RegisterEmail))
                {
                    var error = new ErrorViewModel
                    {
                        ErrorTitle = "Register Failed",
                        ErrorMessage = "Your email is already existed! Try withh another email!"
                    };
                    return View("~/Views/Error/ErrorPage.cshtml", error);
                }
                if(model.RegisterPassword != model.RetypePassword)
                {
                    var error = new ErrorViewModel
                    {
                        ErrorTitle = "Register Failed",
                        ErrorMessage = "Your password and retype password does not match"
                    };
                    return View("~/Views/Error/ErrorPage.cshtml", error);
                }
                var user = new ApplicationUser
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Country = model.Country,
                    CreatedDate = DateTime.Now,
                    Email = model.RegisterEmail,
                    Password = model.RegisterPassword,
                    Phone = model.Phone,
                    RoleID = 2,
                    StatusId = (int) EnumList.UserStatus.Active
                };
                Db.ApplicationUsers.Add(user);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/Account/Index.cshtml");
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
                StatusId = x.StatusId.Value,
                Role = x.UserRole.Label
            }).ToList();

            userInfor.UserList = userList;

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

        [HttpPost]
        public ActionResult UpdateRole(int userId, int roleId)
        {
            var user = Db.ApplicationUsers.Find(userId);
            user.RoleID = roleId;
            if(Db.SaveChanges() > 0 || user.RoleID == roleId)
            {
                return Json("Update Role Successfully!");
            }
            return Json("Update Failed! ");
        }

        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        [HttpPost]
        public ActionResult ResetPassword(LoginRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = model.Email;
                var tempPassword = GetRandomString();

                var user = Db.ApplicationUsers.FirstOrDefault(x => x.Email == userEmail);
                if (user == null)
                {
                    var emailError = new ErrorViewModel
                    {
                        ErrorTitle = "Your email is not existed in the system",
                        ErrorMessage = "Try to log in again or enter another email !"
                    };
                    return View("~/Views/Error/ErrorPage.cshtml", emailError);
                }
                user.Password = tempPassword;
                Db.SaveChanges();
         
                GMailer.GmailUsername = "asset.qscloud@gmail.com";
                GMailer.GmailPassword = "asset2018";

                GMailer mailer = new GMailer();
                mailer.ToEmail = userEmail;
                mailer.Subject = "Reset Password";
                mailer.Body = "Did you forget your password? Don't worry.<br> Here is your temporary password: <br> Temporary Password: " + tempPassword + "<br> Remember to change your password after sign in ! ";
                mailer.IsHtml = true;
                mailer.Send();
                var errorModel = new ErrorViewModel
                {
                    ErrorTitle = "Reset Password Successfully!",
                    ErrorMessage = "Check your personal email to get the temporary password"
                };
                return View("~/Views/Error/ErrorPage.cshtml", errorModel);
            }

            return View("~/Views/Account/Index.cshtml");
        }
    }
}