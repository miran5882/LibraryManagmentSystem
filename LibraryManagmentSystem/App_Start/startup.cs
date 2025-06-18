using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

[assembly: OwinStartup(typeof(LibraryManagementSystem.Startup))]
namespace LibraryManagementSystem
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var issuer = "http://localhost:44305";
            var audience = "http://localhost:44305";
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }
            });
        }
    }
}