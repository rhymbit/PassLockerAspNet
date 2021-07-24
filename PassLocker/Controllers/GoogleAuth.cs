using System;
using System.Data;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace PassLocker.Controllers
{
    [Route("api/[Controller]")]
    public class GoogleAuthController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        
        public GoogleAuthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // GET: api/googleauth/google
        [HttpGet("google")]
        public IActionResult GetGoogle()
        {
            return Accepted();
        }
        
        // POST: api/googleauth/create-user
        [HttpPost("create-user")]
        public async Task<ActionResult> CreateUser([FromBody] Token token)
        {
            try
            {
                GoogleJsonWebSignature.Payload payload =
                    await GoogleJsonWebSignature.ValidateAsync(token.UserToken);
                System.Console.WriteLine(payload.Audience);
                System.Console.WriteLine(payload.Issuer);
                System.Console.WriteLine(payload.ExpirationTimeSeconds);
                if (!payload.Audience.Equals(Configuration["GoogleAuth:ClientId"]))
                {
                    System.Console.WriteLine("Not a valid user");
                    return BadRequest("Invalid User");
                }

                if (!payload.Issuer.Equals("accounts.google.com") &&
                    !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    System.Console.WriteLine("Not a valid user");
                    return BadRequest("Invalid User");
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds)
                        .DateTime;
                    if (now > expiration)
                    {
                        System.Console.WriteLine("Not a valid user");
                        return BadRequest("Invalid User");
                    }
                }
            }
            catch (InvalidJwtException exp)
            {
                System.Console.WriteLine(exp.ToString());
            }

            return Content("Yeah, token accepted");
        }
    }
}