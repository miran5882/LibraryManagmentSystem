using LibraryManagementSystem;
using LibraryManagmentSystem.Models;
using System.Linq;
using System.Threading;
using System.Web.Http;

namespace LibraryManagementSystem.Controllers
{
    [RoutePrefix("api/UserPermissions")]
    public class UserPermissionsController : ApiController
    {
        private readonly LibraryDBEntities db = new LibraryDBEntities();

        [HttpGet]
        [Route("ByRole/{roleId}")]
        [Authorize]
        public IHttpActionResult GetPermissionsByRole(int roleId)
        {
            var currentUserRoleId = GetCurrentUserRoleId();
            if (currentUserRoleId != roleId)
                return Unauthorized();

            var permissions = db.UserPermissions
                .Where(p => p.RoleID == roleId)
                .Select(p => new
                {
                    Resource = p.Resource,
                    Action = p.Action,
                    IsAllowed = p.IsAllowed
                })
                .ToList();

            if (permissions == null || !permissions.Any())
                return NotFound();

            return Ok(permissions);
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
}