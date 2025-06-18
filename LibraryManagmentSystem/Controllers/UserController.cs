using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LibraryManagmentSystem.Controllers
{
    public class UserController : ApiController
    {
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

            // Basic validation
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

                    // Update fields
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.PasswordHash = user.PasswordHash;
                    existingUser.RoleId = user.RoleId;
                    existingUser.IsActive = user.IsActive;
                    existingUser.UpdatedAt = DateTime.Now; // Update the timestamp

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
    }
}