using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LibraryManagmentSystem.Models; 

namespace LibraryManagmentSystem.Controllers
{
    public class BookController : ApiController
    {
        public IEnumerable<Book> Get()
        {
            using(LibraryDBEntities db = new LibraryDBEntities())
            {
                return db.Books.ToList();
            }
        }

        public Book Get(int id)
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                return db.Books.FirstOrDefault(b => b.Id == id);
            }
        }
    }
}
