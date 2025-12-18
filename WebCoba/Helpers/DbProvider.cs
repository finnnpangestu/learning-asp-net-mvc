using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCoba.Models;

namespace WebCoba.Helpers {
    public class DbProvider {
        public static InventoryContext GetContext() {
            var username = System.Web.HttpContext.Current.User.Identity.Name;

            if(string.IsNullOrEmpty(username)) {
                return new InventoryContext("DefaultConnection");
            }

            string dbName = "DB_" + username;
            string connectionString = $@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;AttachDbFilename=|DataDirectory|\{dbName}.mdf";

            return new InventoryContext(connectionString);
        }

    }
}