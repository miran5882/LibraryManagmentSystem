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
public class UserController : ApiController
{
    private readonly LibraryDBEntities db = new LibraryDBEntities();

    [HttpGet]
    public IHttpActionResult Get()
    {
        if (!HasPermission("Users", "Read"))
            return Unauthorized();
        return Ok(db.Users.ToList());
    }

    [HttpGet]
    [Route("{id}")]
    public IHttpActionResult Get(int id)
    {
        if (!HasPermission("Users", "Read"))
            return Unauthorized();
        var user = db.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public IHttpActionResult Post([FromBody] User user)
    {
        if (!HasPermission("Users", "Create"))
            return Unauthorized();
        if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            return BadRequest("Email and PasswordHash are required.");
        db.Users.Add(user);
        db.SaveChanges();
        return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
    }

    [HttpPut]
    [Route("{id}")]
    public IHttpActionResult Put(int id, [FromBody] User user)
    {
        if (!HasPermission("Users", "Update"))
            return Unauthorized();
        if (user == null || id != user.Id)
            return BadRequest();
        var existingUser = db.Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
            return NotFound();
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

    [HttpDelete]
    [Route("{id}")]
    public IHttpActionResult Delete(int id)
    {
        if (!HasPermission("Users", "Delete"))
            return Unauthorized();
        var user = db.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        db.Users.Remove(user);
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