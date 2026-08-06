using Entities.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Model;
using WebApi.Token;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public TokenController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] InputModel Input)
        {
            if (string.IsNullOrWhiteSpace(Input.Email) || string.IsNullOrWhiteSpace(Input.Password))
            {
                return Unauthorized();
            }

            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var token = new TokenJWTBuilder()
                 .AddSecurityKey(JwtSecurityKey.Create(_configuration["Jwt:SecretKey"]!))
                 .AddSubject("joao master")
                 .AddIssuer(_configuration["Jwt:Issuer"]!)
                 .AddAudience(_configuration["Jwt:Audience"]!)
                 .AddClaim("UsuarioAPINumero", "1")
                 .AddExpiry(5)
                 .Builder();

                return Ok(token.value);

            }
            else
            {
                return Unauthorized();
            }

        }
    }
}
