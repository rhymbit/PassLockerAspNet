using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PassLocker.Dto;
using PassLocker.Services.Protector;
using PassLocker.Services.Token;
using PassLockerDatabase;

namespace PassLocker.Controllers
{
    [ApiController]
    [Route("api/[Controller]/{id:int}")]
    public class PasswordController : ControllerBase
    {
        private readonly PassLockerDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IProtector _protector;

        public PasswordController(PassLockerDbContext dbContext, ITokenService tokenService, IProtector protector)
        {
            this._db = dbContext;
            this._tokenService = tokenService;
            this._protector = protector;
        }

        // Remove this method from production
        // GET: api/password/test-token/{id}
        [HttpGet("test-token")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetTestToken(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }

            var token = _tokenService.CreateToken(user.UserName, user.UserPasswordHash);

            return Ok(token);
        }

        // GET: api/password/{id}
        [HttpPost("verify-token")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyToken(int id, [FromBody] Tokens token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not structured properly in the header");
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("Invalid user's id. No such user exists");
            }

            var isValid = _tokenService.ValidateToken(token.PasswordToken, user.UserSecretHash);
            if (!isValid)
            {
                return BadRequest("Token is not valid or has expired");
            }

            return NoContent();
        }

        [HttpPost("verify-user")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<string>> VerifyUser(int id, [FromBody] VerifyUserDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("User's credentials are not in correctly organised in the payload.");
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User doesn't exist.");
            }

            var isPasswordValid = _protector.VerifyHashing(
                credentials.password, user.UserPasswordHash, user.UserPasswordSalt);
            if (!isPasswordValid)
            {
                return Unauthorized("Incorrect password");
            }

            var isSecretValid = _protector.VerifyHashing(
                credentials.secret, user.UserSecretHash, user.UserSecretSalt);
            if (!isSecretValid)
            {
                return Unauthorized("Incorrect secret answer");
            }

            var token = _tokenService.CreateToken(user.UserName, user.UserSecretHash);
            
            return Ok(token);
        }

        [HttpPost("create-passwords")]
        public async Task<IActionResult> CreatePasswords(int id, [FromBody] Dictionary<string, string> passwords)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Password's data payload is not correct.");
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User dose not exist.");
            }

            foreach (var password in passwords)
            {
                var userPassword = CreateUserPassword(id, password.Key, password.Value);
                user.Passwords.Add(userPassword);
            }
            
            return NoContent();
        }

        private static UserPassword CreateUserPassword(int userId, string domain, string password)
        {
            var protector = new Protector();
            
            var (passwordHash, salt) = protector.CreateHashedStringAndSalt(password);
            
            var userPassword = new UserPassword()
            {
                DomainName = domain,
                PasswordSalt = salt,
                PasswordHash = passwordHash,
                UserId = userId
            };

            return userPassword;
        }
    }
}