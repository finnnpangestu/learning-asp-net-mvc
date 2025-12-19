using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCoba.Models;

namespace WebCoba.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AccountController() {
            var db = new InventoryContext("DefaultConnection");

            var userStore = new UserStore<ApplicationUser>(db);
            _userManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<IdentityRole>(db);
            _roleManager = new RoleManager<IdentityRole>(roleStore);
        }

        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string password) {
            var user = new ApplicationUser { UserName = username };
            var result = _userManager.Create(user, password);

            if(result.Succeeded) {
                if(!_roleManager.RoleExists("User")) {
                    _roleManager.Create(new IdentityRole("User"));
                }

                _userManager.AddToRole(user.Id, "User");

                DoSignIn(user);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Errors.FirstOrDefault());
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl) {
            var user = _userManager.Find(username, password);

            if(user != null) {
                DoSignIn(user);

                if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                    return Redirect(returnUrl);
                }

                var roles = _userManager.GetRoles(user.Id);
                if (roles.Contains("Admin")) {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Username atau password salah.");
            return View();
        }

        private void DoSignIn(ApplicationUser user) {
            var authManager = HttpContext.GetOwinContext().Authentication;

            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

            authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout() {
            var authManager = HttpContext.GetOwinContext().Authentication;

            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }
    }
}