using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCoba.Helpers;
using WebCoba.Models;

namespace WebCoba.Controllers {
    public class HomeController : Controller {
        private InventoryContext db;

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            db = DbProvider.GetContext();

            base.OnActionExecuting(filterContext);
        }

        // GET: Inventory
        public ActionResult Index(string search, string sortOrder, int page = 1) {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = sortOrder;

            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSort = sortOrder == "Price" ? "price_desc" : "Price";

            var items = db.Products.AsQueryable();

            if(!string.IsNullOrEmpty(search)) {
                items = items.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            switch(sortOrder) {
                case "name_desc":
                items = items.OrderByDescending(x => x.Name);
                break;
                case "Price":
                items = items.OrderBy(x => x.Price);
                break;
                case "price_desc":
                items = items.OrderByDescending(x => x.Price);
                break;
                default:
                items = items.OrderBy(x => x.Name);
                break;
            }

            int pageSize = 5;
            int totalData = items.Count();

            var dataPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalData / pageSize);
            ViewBag.currentPage = page;

            if(Request.IsAjaxRequest()) {
                return PartialView("_InventoryTable", dataPerPage);
            }

            return View(dataPerPage);
        }

        public ActionResult Details(int id) {
            var item = db.Products.FirstOrDefault(x => x.Id == id);

            if(item == null) return HttpNotFound();

            return View(item);
        }

        // GET: Create
        public ActionResult Create() {
            return View();
        }

        // POST: Save Inventory
        [HttpPost]
        public ActionResult Create(Product value) {
            if (ModelState.IsValid) {
                db.Products.Add(value);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(value);
        }

        // DELETE: Delete Inventory
        [HttpPost]
        public ActionResult Delete(int id) {
            var product = db.Products.Find(id);
            
            if(product != null) {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return Index(null, null, 1);
        }

        // GET: Inventory
        public ActionResult Edit(int id) {
            var product = db.Products.FirstOrDefault(x => x.Id == id);

            if(product == null) {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Update Inventory
        [HttpPost]
        public ActionResult Edit(Product newValue) {
            if (ModelState.IsValid) {
                try {
                    db.Entry(newValue).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                } catch (Exception e) {
                    ModelState.AddModelError("", "Gagal menyimpan: " + e.Message);
                }
            }

            return View(newValue);
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(db != null) {
                    db.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}