using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibraryManagmentSystem.Controllers
{
    public class RoleController : ApiController
    {
        public IEnumerable<Role> Get()
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                return db.Roles.ToList();
            }
        }

        public Role Get(int id)
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                return db.Roles.FirstOrDefault(r => r.Id == id);
            }
        }
    }
}
