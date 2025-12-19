using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCoba.Models;

namespace WebCoba.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private InventoryContext dbMaster = new InventoryContext("DefaultConnection");

        public ActionResult Index(string search) {
            var query = dbMaster.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(u => u.UserName.Contains(search));
            }

            var userList = query.ToList().Select(u => new UserView {
                UserId = u.Id,
                Username = u.UserName,
                RoleName = GetUserRoleName(u.Id)
            }).ToList();

            ViewBag.CurrentSearch = search;
            return View(userList.ToList());
        }

        public ActionResult ViewUserInventory(string targetUsername) {
            if(string.IsNullOrEmpty(targetUsername)) {
                return RedirectToAction("Index");
            }

            
            string connextionString = $@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=DB_{targetUsername};Integrated Security=True;AttachDbFilename=|DataDirectory|\DB_{targetUsername}.mdf";

            try {
                using(var userDb = new InventoryContext(connextionString)) {
                    var dataBarang = userDb.Products.ToList();

                    ViewBag.TargetUser = targetUsername;
                    ViewBag.DbName = "DB_" + targetUsername;

                    return View(dataBarang);
                }
            } catch(Exception e) {
                TempData["ErrorMessage"] = "Database untuk user " + targetUsername + " belum tersedia." + e.ToString();
                return RedirectToAction("Index");
            }
        }

        private string GetUserRoleName(string userId) {
            var db = new InventoryContext("DefaultConnection");
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var roles = userManager.GetRoles(userId);
            return roles.FirstOrDefault() ?? "User";
        }

        protected override void Dispose(bool disposing) {
            if (disposing) dbMaster.Dispose();

            base.Dispose(disposing);
        }
    }
}