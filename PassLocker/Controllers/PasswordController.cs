using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("create-passwords")]
        public async Task<IActionResult> CreatePasswords(string id, [FromBody] Dictionary<string, string> passwords)
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
            
            // get all stored passwords
            var userPasswords = user.Passwords;
            Console.WriteLine(userPasswords.Count);

            foreach (var pass in userPasswords)
            {
                Console.WriteLine(pass);
            }

            foreach (var storedPassword in userPasswords)
            {
                var domainName = storedPassword.DomainName;
                foreach (var providedPassword in passwords)
                {
                    // if any of the password's domain name already exists
                    // then we just have to update the password
                    if (domainName.Equals(providedPassword.Key)) // Key is the domain name, Value is the password
                    {
                        var newEntry = CreateUserPassword(
                            user.UserId, domainName, providedPassword.Value, user.UserPasswordSalt);
                        // must use the same 'id' value to update changes instead of creating a new one
                        newEntry.UserPasswordId = storedPassword.UserPasswordId;
                        _db.UserPasswords.Update(newEntry);
                        
                        // remove this key-value pair from the dictionary
                        passwords.Remove(providedPassword.Key);
                    }
                }
            }
            
            // create a new entry for rest of the passwords
            foreach (var password in passwords)
            {
                var userPassword = CreateUserPassword(id, password.Key, password.Value, user.UserPasswordSalt);
                user.Passwords.Add(userPassword);
                await _db.UserPasswords.AddAsync(userPassword);
                _db.Users.Update(user);
            }

            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
            {
                return NoContent();
            }

            return Problem("Problem at the server. Cannot create password.");
        }

        private UserPassword CreateUserPassword(string userId, string domain, string domainPassword, string salt)
        {
            var encryptedPassword = _protector.EncryptData(
                domainPassword, "MySecretPassword", salt);

            var userPassword = new UserPassword()
            {
                DomainName = domain,
                PasswordSalt = salt,
                PasswordHash = encryptedPassword,
                UserId = userId
            };
            
            // creating a unique uuid for password
            var uuid = _protector.GetUuid();
            userPassword.UserPasswordId = uuid;

            return userPassword;
        }
    }
}