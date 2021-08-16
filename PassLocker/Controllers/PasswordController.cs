using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        
        private const string MySecretPassword = "MySecretPassword";

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
        public async Task<IActionResult> GetTestToken(string id)
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
        public async Task<IActionResult> VerifyToken(string id, [FromBody] Tokens token)
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
        public async Task<ActionResult<string>> VerifyUser(string id, [FromBody] VerifyUserDto credentials)
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

        [HttpGet("get-passwords")]
        public async Task<ActionResult<List<UserPasswordsDto>>> GetPasswords(string id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return BadRequest("User does not exits");
            }

            await _db.Entry(user)
                .Collection(u => u.Passwords)
                .LoadAsync();

            var passwords = new List<UserPasswordsDto>();

            foreach (var pass in user.Passwords)
            {
                var decryptedPassword = _protector.DecryptData(
                    pass.PasswordHash, MySecretPassword, pass.PasswordSalt);
                passwords.Add(PasswordsToDto(pass.DomainName, decryptedPassword));
            }

            return Ok(passwords);
        }

        [HttpPost("create-passwords")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserPassword>> CreatePasswords(string id,
            [FromBody] Dictionary<string, string> providedPasswords)
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
                            sp.DomainName, pp.Value, sp.PasswordSalt, sp.UserPasswordId);
                        passwordsToUpdate.Add(updatedPassword);

                        // removing the old entry
                        _db.UserPasswords.Remove(sp);
                    }
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
            if (affected > 0)
            {
                return NoContent();
            }

            return Problem("Problem at the server. Cannot create password.");
        }

        

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

        private UserPasswordsDto PasswordsToDto(string domain, string password)
            => new UserPasswordsDto()
            {
                Domain = domain,
                Password = password
            };
    }
}