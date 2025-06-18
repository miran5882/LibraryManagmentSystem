using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("RoleName is required.");
            }

            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (db.Roles.Any(r => r.RoleName == role.RoleName))
                    {
                        return BadRequest("A role with this name already exists.");
                    }

                    role.Id = 0;
                    role.CreatedAt = DateTime.Now;
                    role.UpdatedAt = DateTime.Now;

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

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest("Role data is null.");
            }

            if (id != role.Id)
            {
                return BadRequest("Id mismatch between route and body.");
            }

            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("RoleName is required.");
            }

            try
            {
                using (LibraryDBEntities db = new LibraryDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var existingRole = db.Roles.FirstOrDefault(r => r.Id == id);
                    if (existingRole == null)
                    {
                        return NotFound();
                    }

                    existingRole.RoleName = role.RoleName;
                    existingRole.Description = role.Description;
                    existingRole.UpdatedAt = role.UpdatedAt != default(DateTime) ? role.UpdatedAt : DateTime.Now;

                    db.SaveChanges();
                    return Ok(existingRole);
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

                    var role = db.Roles.FirstOrDefault(r => r.Id == id);
                    if (role == null)
                    {
                        return NotFound();
                    }

                    db.Roles.Remove(role);
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