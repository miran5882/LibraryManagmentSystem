using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LibraryManagmentSystem.Controllers
{
    public class UserController : ApiController
    {
        // GET api/User
        [Authorize(Roles = "Admin, In-Charge")]
        public IEnumerable<User> Get()
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                return db.Users.ToList();
            }
        }

        public User Get(int id)
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                return db.Users.FirstOrDefault(u => u.Id == id);
            }
        }

        // POST api/User
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is null.");
            }

            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName) ||
                string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("FirstName, LastName, Email, and Password are required.");
            }

            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (db.Users.Any(u => u.Email == user.Email))
                    {
                        return BadRequest("A user with this email already exists.");
                    }

                    if (!db.Roles.Any(r => r.Id == user.RoleId))
                    {
                        return BadRequest("Invalid RoleId.");
                    }

                    user.Id = 0;
                    if (user.CreatedAt == default(DateTime))
                    {
                        user.CreatedAt = DateTime.Now;
                    }
                    if (user.UpdatedAt == default(DateTime))
                    {
                        user.UpdatedAt = DateTime.Now;
                    }

                    db.Users.Add(user);
                    db.SaveChanges();
                    return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
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
        // PUT api/User/id
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is null.");
            }

            if (id != user.Id)
            {
                return BadRequest("Id mismatch between route and body.");
            }

            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName) ||
                string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("FirstName, LastName, Email, and Password are required.");
            }

            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var existingUser = db.Users.FirstOrDefault(u => u.Id == id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.PasswordHash = user.PasswordHash;
                    existingUser.RoleId = user.RoleId;
                    existingUser.IsActive = user.IsActive;
                    existingUser.UpdatedAt = DateTime.Now;

                    db.SaveChanges();
                    return Ok(existingUser);
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
        // DELETE api/User/id
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var user = db.Users.FirstOrDefault(u => u.Id == id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    db.Users.Remove(user);
                    db.SaveChanges();
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        protected override void Dispose(bool disposing)
        {
            using (LibraryDBEntities db = new LibraryDBEntities())
                if (disposing)
                {
                    db.Dispose();
                }
            base.Dispose(disposing);
        }
    }
}