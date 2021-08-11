using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PassLocker.Services.Token;
using PassLockerDatabase;

namespace PassLocker.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PasswordController : ControllerBase
    {
        private PassLockerDbContext db;
        private ITokenService tokenService;

        public PasswordController(PassLockerDbContext dbContext, ITokenService tokenSer)
        {
            db = dbContext;
            tokenService = tokenSer;
        }
        
        // GET: api/password/test-token/{id}
        [HttpGet("test-token/{id:int}")]
        public async Task<IActionResult> GetTestToken(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }
            
            var token = tokenService.CreateToken(user.UserName, user.UserPasswordHash);

            return Ok(token);
        }
        
        // GET: api/password/{id}
        [HttpPost("{id:int}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPermission(int id, [FromBody] Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not structured properly in the header");
            }
            
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("Invalid user's id. No such user exists");
            }
            
            var isValid = tokenService.ValidateToken(token.PasswordToken, user.UserPasswordHash);
            if (!isValid)
            {
                return BadRequest("Token is not valid or has expired");
            }

            return Ok("Token is valid");
        }
    }
}