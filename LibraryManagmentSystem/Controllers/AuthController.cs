using LibraryManagementSystem;
using LibraryManagmentSystem.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

[RoutePrefix("api/auth")]
public class AuthController : ApiController
{
    private readonly LibraryDBEntities db = new LibraryDBEntities();

    [AllowAnonymous]
    [HttpPost]
    [Route]
    public IHttpActionResult Authenticate([FromBody] CredentialModel credentials)
    {
        if (credentials == null || string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
            return BadRequest("Email and password are required.");

        var user = db.Users.FirstOrDefault(u => u.Email == credentials.Email && u.PasswordHash == credentials.Password);
        if (user == null)
            return Unauthorized();

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"]));
        var credentialsKey = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("RoleId", user.RoleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "https://localhost:44305",
            audience: "https://localhost:44305",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentialsKey
        );

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