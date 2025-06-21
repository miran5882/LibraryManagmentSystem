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
public class RoleController : ApiController
{
    private readonly LibraryDBEntities db = new LibraryDBEntities();

    [HttpGet]
    public IHttpActionResult Get()
    {
        if (!HasPermission("Roles", "Read"))
            return Unauthorized();
        return Ok(db.Roles.ToList());
    }

    [HttpGet]
    [Route("{id}")]
    public IHttpActionResult Get(int id)
    {
        if (!HasPermission("Roles", "Read"))
            return Unauthorized();
        var role = db.Roles.FirstOrDefault(r => r.Id == id);
        if (role == null)
            return NotFound();
        return Ok(role);
    }

    [HttpPost]
    public IHttpActionResult Post([FromBody] Role role)
    {
        if (!HasPermission("Roles", "Create"))
            return Unauthorized();
        if (role == null || string.IsNullOrEmpty(role.RoleName))
            return BadRequest("RoleName is required.");
        db.Roles.Add(role);
        db.SaveChanges();
        return CreatedAtRoute("DefaultApi", new { id = role.Id }, role);
    }

    [HttpPut]
    [Route("{id}")]
    public IHttpActionResult Put(int id, [FromBody] Role role)
    {
        if (!HasPermission("Roles", "Update"))
            return Unauthorized();
        if (role == null || id != role.Id)
            return BadRequest();
        var existingRole = db.Roles.FirstOrDefault(r => r.Id == id);
        if (existingRole == null)
            return NotFound();
        existingRole.RoleName = role.RoleName;
        existingRole.Description = role.Description;
        db.SaveChanges();
        return Ok(existingRole);
    }

    [HttpDelete]
    [Route("{id}")]
    public IHttpActionResult Delete(int id)
    {
        if (!HasPermission("Roles", "Delete"))
            return Unauthorized();
        var role = db.Roles.FirstOrDefault(r => r.Id == id);
        if (role == null)
            return NotFound();
        db.Roles.Remove(role);
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