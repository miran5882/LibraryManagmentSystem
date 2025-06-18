using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibraryManagmentSystem.Controllers
{
    public class UserController : ApiController
    {
        public IEnumerable<User> Get()
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false; // Disable proxy creation
                return db.Users.ToList();
            }
        }

        public User Get(int id)
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false; // Disable proxy creation
                return db.Users.FirstOrDefault(u => u.Id == id);
            }
        }
    }
}
