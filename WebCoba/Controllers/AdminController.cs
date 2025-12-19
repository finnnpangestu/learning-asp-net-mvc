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

        public ActionResult Index(string search, string sortOrder, int page = 1) {
            var userStore = new UserStore<ApplicationUser>(dbMaster);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var query = dbMaster.Users.AsQueryable();

            if(!string.IsNullOrEmpty(search)) {
                query = query.Where(u => u.UserName.Contains(search));
            }

            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CurrentSort = sortOrder;

            switch(sortOrder) {
                case "name_desc": query = query.OrderByDescending(u => u.UserName); break;
                default: query = query.OrderBy(u => u.UserName); break;
            }

            int pageSize = 5;
            int totalItems = query.Count();
            var pagedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var userList = pagedData.Select(u => new UserView {
                UserId = u.Id,
                Username = u.UserName,
                RoleName = GetUserRoleName(u.Id),
                IsSuspended = userManager.IsLockedOut(u.Id)
            }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentSearch = search;

            return View(userList);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuspendUser(string userId, int? days) {
            if (string.IsNullOrEmpty(userId)) {
                return HttpNotFound();
            }

            var db = new InventoryContext("DefaultConnection");
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var user = userManager.FindById(userId);

            userManager.SetLockoutEnabled(userId, true);

            if (days == -1) {
                userManager.SetLockoutEndDate(userId, DateTimeOffset.MaxValue);
                TempData["SuccessMessage"] = "User berhasil di-suspend permanen.";
            } else {
                var endDate = DateTimeOffset.UtcNow.AddDays((double)days.Value);

                userManager.SetLockoutEndDate(userId, endDate);
                TempData["SuccessMessage"] = $"User berhasil di-suspend selama {days} hari.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnsuspendUser(string userId) {
            if(string.IsNullOrEmpty(userId)) return HttpNotFound();

            var db = new InventoryContext("DefaultConnection");
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.SetLockoutEndDate(userId, DateTimeOffset.UtcNow.AddMinutes(-1));

            TempData["Success"] = "Akses pengguna telah dipulihkan.";
            return RedirectToAction("Index");
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