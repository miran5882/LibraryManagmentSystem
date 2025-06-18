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
            using (LibraryDBEntities db = new LibraryDBEntities())
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

        [HttpPost]
        public IHttpActionResult Post([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Book data is null.");
            }

            // Basic validation
            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author) || string.IsNullOrEmpty(book.ISBN))
            {
                return BadRequest("Title, Author, and ISBN are required.");
            }

            if (book.Total_Copies < 0 || book.Available_Copies < 0 || book.Available_Copies > book.Total_Copies)
            {
                return BadRequest("Invalid copy counts.");
            }

            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false; // Disable proxy creation

                    // Ensure CreatedAt and UpdatedAt are set if not provided
                    if (book.CreatedAt == default(DateTime))
                    {
                        book.CreatedAt = DateTime.Now;
                    }
                    if (book.UpdatedAt == default(DateTime))
                    {
                        book.UpdatedAt = DateTime.Now;
                    }

                    // Ensure Id is not set (let the database generate it)
                    book.Id = 0; // EF will ignore this and let the database generate the Id

                    db.Books.Add(book);
                    db.SaveChanges();
                    return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Capture validation errors
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                var fullErrorMessage = "Validation failed: " + string.Join("; ", errorMessages);
                return BadRequest(fullErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
