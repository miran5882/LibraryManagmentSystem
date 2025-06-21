using LibraryManagementSystem;
using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;

[EnableCors(origins: "http://localhost:44305", headers: "*", methods: "*")]
public class BookController : ApiController
{
    private readonly LibraryDBEntities db = new LibraryDBEntities();

    [HttpGet]
    public IHttpActionResult Get()
    {
        if (!HasPermission("Books", "Read"))
            return Unauthorized();
        var books = db.Books
            .Include("Category")
            .Include("Publication")
            .Select(b => new
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author, 
                ISBN = b.ISBN,
                PublicationDate = b.PublicationDate,
                AvailabilityStatus = b.IsActive,
                Category = b.Category,
                Publisher = b.Publication.PublisherName,
                PublicationYear = b.Publication.PublicationYear
            })
            .ToList();
        return Ok(books);
    }

    [HttpGet]
    [Route("{id}")]
    public IHttpActionResult Get(int id)
    {
        if (!HasPermission("Books", "Read"))
            return Unauthorized();
        var book = db.Books
            .Include("Category")
            .Include("Publication")
            .FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound();
        return Ok(new
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            PublicationDate = book.PublicationDate,
            AvailabilityStatus = book.IsActive,
            Category = book.Category,
            Publisher = book.Publication.PublisherName,
            PublicationYear = book.Publication.PublicationYear
        });
    }

    [HttpPost]
    public IHttpActionResult Post([FromBody] Book book)
    {
        if (!HasPermission("Books", "Create"))
            return Unauthorized();
        if (book == null || string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.ISBN))
            return BadRequest("Title and ISBN are required.");
        db.Books.Add(book);
        db.SaveChanges();
        return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
    }

    [HttpPut]
    [Route("{id}")]
    public IHttpActionResult Put(int id, [FromBody] Book book)
    {
        if (!HasPermission("Books", "Update"))
            return Unauthorized();
        if (book == null || id != book.Id)
            return BadRequest();
        var existingBook = db.Books.FirstOrDefault(b => b.Id == id);
        if (existingBook == null)
            return NotFound();
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.ISBN = book.ISBN;
        existingBook.PublicationDate = book.PublicationDate;
        existingBook.IsActive = book.IsActive;
        existingBook.CategoryID = book.CategoryID;
        existingBook.PublicationID = book.PublicationID;
        db.SaveChanges();
        return Ok(existingBook);
    }

    [HttpDelete]
    [Route("{id}")]
    public IHttpActionResult Delete(int id)
    {
        if (!HasPermission("Books", "Delete"))
            return Unauthorized();
        var book = db.Books.FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound();
        db.Books.Remove(book);
        db.SaveChanges();
        return StatusCode(HttpStatusCode.NoContent);
    }

    private bool HasPermission(string resource, string action)
    {
        var roleId = GetCurrentUserRoleId();
        return db.UserPermissions.Any(p => p.RoleID == roleId && p.Resource == resource && p.Action == action && p.IsAllowed);
    }

    private int GetCurrentUserRoleId()
    {
        var identity = (System.Security.Claims.ClaimsPrincipal)Thread.CurrentPrincipal;
        var roleIdClaim = identity.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;
        return string.IsNullOrEmpty(roleIdClaim) ? 0 : int.Parse(roleIdClaim);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}