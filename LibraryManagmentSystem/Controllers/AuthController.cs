using LibraryManagementSystem;
using LibraryManagmentSystem.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

[EnableCors(origins: "http://localhost:44305", headers: "*", methods: "*")]
public class AuthController : ApiController
{
    private readonly LibraryDBEntities db = new LibraryDBEntities();

    [AllowAnonymous]
    [HttpPost]
    [Route("api/auth")]
    public IHttpActionResult Authenticate([FromBody] CredentialModel credentials)
    {
        if (credentials == null || string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var user = db.Users.FirstOrDefault(u => u.Email == credentials.Email && u.PasswordHash == credentials.Password);
        if (user == null)
        {
            return Unauthorized();
        }

        var issuer = "http://localhost:44305";
        var audience = "http://localhost:44305";
        var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
        var key = Encoding.ASCII.GetBytes(secretKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentialsKey = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roleName = db.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.RoleName ?? "User";
        var claims = new[]
        {
               new Claim(ClaimTypes.Name, user.Email),
               new Claim(ClaimTypes.Role, roleName)
           };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentialsKey);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { token = tokenString });
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

public class CredentialModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}