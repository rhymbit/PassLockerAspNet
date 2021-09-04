using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PassLocker.Dto;
using PassLocker.Services.Protector;
using PassLocker.Services.Token;
using PassLockerDatabase;

namespace PassLocker.Controllers
{
    [ApiController]
    [Route("api/[Controller]/{id}")]
    public class PasswordController : ControllerBase
    {
        private readonly PassLockerDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IProtector _protector;
        private IConfiguration Configuration { get; }

        private string MySecretPassword { get; }

        public PasswordController(PassLockerDbContext dbContext, ITokenService tokenService, IProtector protector,
            IConfiguration configuration)
        {
            _db = dbContext;
            _tokenService = tokenService;
            _protector = protector;
            Configuration = configuration;

            MySecretPassword = Configuration["ENCRYPTION_PASSWORD"]; // remove this later
        }

        // POST: api/password/{id}/verify-token
        [HttpPost("verify-token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> VerifyToken(string id, [FromBody] Tokens token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exist");
            }

            var isValid = _tokenService.ValidateToken(token.PasswordToken, user.UserSecretHash);
            if (!isValid)
            {
                return Unauthorized("Invalid or expired token");
            }

            return Ok("Token Valid");
        }

        // POST: api/password/{id}/verify-user
        [HttpPost("verify-user")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> VerifyUser(string id, [FromBody] VerifyUserDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exists");
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

        // This method/endpoint should not be exposed to client
        // POST: api/password/{id}/get-passwords
        [HttpGet("get-passwords")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dictionary<string,string>>> GetPasswords(string id, [FromQuery] string token)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exits");
            }

            var isTokenValid = _tokenService.ValidateToken(token, user.UserSecretHash);
            if (!isTokenValid)
            {
                return Unauthorized("Invalid or expired token");
            }

            await _db.Entry(user)
                .Collection(u => u.Passwords)
                .LoadAsync();

            var passwordsDto = GetPasswords(user);

            return Ok(passwordsDto.ToDictionary(
                pass => pass.Domain, 
                pass => pass.Password));
        }


        // POST: api/password/{id}/create-passwords
        [HttpPost("create-passwords")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dictionary<string,string>>> CreatePasswords(string id,
            [FromBody] Dictionary<string, string> providedPasswords, [FromQuery] string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exist");
            }
            
            var isTokenValid = _tokenService.ValidateToken(token, user.UserSecretHash);
            if (!isTokenValid)
            {
                return Unauthorized("Invalid or expired token");
            }
        
            // explicitly loading all passwords
            await _db.Entry(user)
                .Collection(u => u.Passwords)
                .LoadAsync();
        
            // list to store any to-be-updated passwords
            var passwordsToUpdate = new List<UserPassword>();
        
            // finding and storing any to-be-updated passwords in `passwordsToUpdate` list
            foreach (var sp in user.Passwords)
            {
                string popOff = null;
        
                foreach (var pp in providedPasswords)
                {
                    if (sp.DomainName.Equals(pp.Key))
                    {
                        popOff = pp.Key;
                        var updatedPassword = CreateUserPassword(user.UserId,
                            sp.DomainName, pp.Value, sp.PasswordSalt);
                        passwordsToUpdate.Add(updatedPassword);
                    }
        
                    // removing the old password, because
                    // 1. either user has updated the password ( `if` condition above )
                    // 2. or user has deleted the password at the frontend
                    _db.UserPasswords.Remove(sp);
                }
        
                // pop-off the updated value from `providedPassword`
                // so later a new password value is not generated for this entry
                if (popOff != null)
                {
                    providedPasswords.Remove(popOff);
                }
            }
        
            // update password if any needs to be updated
            if (passwordsToUpdate.Count > 0)
            {
                await _db.UserPasswords.AddRangeAsync(passwordsToUpdate);
            }
        
            // create a new entry for new values in `providedPasswords`
            foreach (var password in providedPasswords)
            {
                var userPassword = CreateUserPassword(id, password.Key, password.Value, user.UserPasswordSalt);
                await _db.UserPasswords.AddAsync(userPassword);
            }
        
            var affected = await _db.SaveChangesAsync();
        
            if (affected == 0)
            {
                return NoContent(); // no passwords were created or updated
            }
        
            // else send out the passwords
            var passwordsDto = GetPasswords(user);
            return Ok(passwordsDto.ToDictionary(
                pass => pass.Domain, 
                pass => pass.Password));
        }
        
        
        // Utility Method
        private UserPassword CreateUserPassword(string userId,
            string domain, string domainPassword, string salt, string passwordId = null)
        {
            var encryptedPassword = _protector.EncryptData(
                domainPassword, MySecretPassword, salt);

            var userPassword = new UserPassword()
            {
                UserId = userId,
                DomainName = domain,
                PasswordHash = encryptedPassword,
                PasswordSalt = salt,
            };

            if (passwordId == null)
            {
                // creating a unique uuid for password
                var uuid = _protector.GetUuid();
                userPassword.UserPasswordId = uuid;
            }
            else
            {
                userPassword.UserPasswordId = passwordId;
            }

            return userPassword;
        }
        
        
        // Utility Method
        private IEnumerable<UserPasswordsDto> GetPasswords(User user)
        {
            var passwordsDto = user.Passwords.Select(pass => new UserPasswordsDto
            {
                Domain = pass.DomainName,
                Password = _protector.DecryptData(pass.PasswordHash, MySecretPassword, pass.PasswordSalt)
            }).ToArray();
            return passwordsDto;
        }
    }
}