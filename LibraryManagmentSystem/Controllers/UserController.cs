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
                    db.Configuration.ProxyCreationEnabled = false; // Disable proxy creation

                    // Check if Email already exists (since Email is UNIQUE in the database)
                    if (db.Users.Any(u => u.Email == user.Email))
                    {
                        return BadRequest("A user with this email already exists.");
                    }

                    // Check if RoleId exists
                    if (!db.Roles.Any(r => r.Id == user.RoleId))
                    {
                        return BadRequest("Invalid RoleId.");
                    }

                    // Ensure Id is not set (let the database generate it)
                    user.Id = 0;

                    // Ensure CreatedAt and UpdatedAt are set if not provided
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
    }
}