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
                    db.Configuration.ProxyCreationEnabled = false;

                    if (book.CreatedAt == default(DateTime))
                    {
                        book.CreatedAt = DateTime.Now;
                    }
                    if (book.UpdatedAt == default(DateTime))
                    {
                        book.UpdatedAt = DateTime.Now;
                    }

                    book.Id = 0;

                    db.Books.Add(book);
                    db.SaveChanges();
                    return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
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

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Book data is null.");
            }

            if (id != book.Id)
            {
                return BadRequest("Id mismatch between route and body.");
            }

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
                    db.Configuration.ProxyCreationEnabled = false;

                    var existingBook = db.Books.FirstOrDefault(b => b.Id == id);
                    if (existingBook == null)
                    {
                        return NotFound();
                    }

                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.ISBN = book.ISBN;
                    existingBook.Publication_Year = book.Publication_Year;
                    existingBook.Category = book.Category;
                    existingBook.Total_Copies = book.Total_Copies;
                    existingBook.Available_Copies = book.Available_Copies;
                    existingBook.Description = book.Description;
                    existingBook.IsActive = book.IsActive;
                    existingBook.UpdatedAt = book.UpdatedAt != default(DateTime) ? book.UpdatedAt : DateTime.Now;

                    db.SaveChanges();
                    return Ok(existingBook);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
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

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var book = db.Books.FirstOrDefault(b => b.Id == id);
                    if (book == null)
                    {
                        return NotFound();
                    }

                    db.Books.Remove(book);
                    db.SaveChanges();
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
