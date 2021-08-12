using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PassLocker.Dto;
using PassLocker.Services.Protector;
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
        private IProtector protector;

        public PasswordController(PassLockerDbContext dbContext, ITokenService tokenSer, IProtector protector)
        {
            this.db = dbContext;
            this.tokenService = tokenSer;
            this.protector = protector;
        }

        // Remove this method from production
        // GET: api/password/test-token/{id}
        [HttpGet("test-token/{id:int}")]
        [ProducesResponseType(200, Type = typeof(Token))]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyToken(int id, [FromBody] Token token)
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

            var isValid = tokenService.ValidateToken(token.PasswordToken, user.UserSecretAnswerHash);
            if (!isValid)
            {
                return BadRequest("Token is not valid or has expired");
            }

            return NoContent();
        }

        [HttpPost("{id:int}/verify-user")]
        [ProducesResponseType(200, Type = typeof(Token))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Token>> VerifyUser(int id, [FromBody] ViewPasswordsDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("User's credentials are not in correctly organised in the payload.");
            }

            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User doesn't exist.");
            }

            var isPasswordValid = protector.VerifyHashing(
                credentials.password, user.UserPasswordHash, user.UserPasswordSalt);
            if (!isPasswordValid)
            {
                return Unauthorized("Incorrect password");
            }

            var isSecretValid = protector.VerifyHashing(
                credentials.secret, user.UserSecretAnswerHash, user.UserSecretSalt);
            if (!isSecretValid)
            {
                return Unauthorized("Incorrect secret answer");
            }

            var newToken = new Token
            {
                GoogleToken = "",
                PasswordToken = tokenService.CreateToken(user.UserName, user.UserSecretAnswerHash)
            };

            return Ok(newToken);
        }
    }
}