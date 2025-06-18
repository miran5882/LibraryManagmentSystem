using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                db.Configuration.ProxyCreationEnabled = false;
                return db.Roles.FirstOrDefault(r => r.Id == id);
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest("Role data is null.");
            }

            // Basic validation
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("RoleName is required.");
            }

            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false; // Disable proxy creation

                    // Check if RoleName already exists (since RoleName is UNIQUE in the database)
                    if (db.Roles.Any(r => r.RoleName == role.RoleName))
                    {
                        return BadRequest("A role with this name already exists.");
                    }

                    // Ensure Id is not set (let the database generate it)
                    role.Id = 0;

                    // Ensure CreatedAt and UpdatedAt are set if not provided
                    if (role.CreatedAt == default(DateTime))
                    {
                        role.CreatedAt = DateTime.Now;
                    }
                    if (role.UpdatedAt == default(DateTime))
                    {
                        role.UpdatedAt = DateTime.Now;
                    }

                    db.Roles.Add(role);
                    db.SaveChanges();
                    return CreatedAtRoute("DefaultApi", new { id = role.Id }, role);
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