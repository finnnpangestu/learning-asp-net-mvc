using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebCoba.Models {
    public class InventoryContext : IdentityDbContext<ApplicationUser> {
        public InventoryContext() : base("DefaultConnection") { }

        public InventoryContext(string dbName) : base(dbName) {
            Database.SetInitializer(new CreateDatabaseIfNotExists<InventoryContext>());
        }

        public DbSet<Product> Products { get; set; }
    }
}