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
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly PassLockerDbContext _db;
        private readonly IProtector _protector;
        private readonly ITokenService _tokenService;

        public UserController(PassLockerDbContext dbContext, IProtector protector, ITokenService tokenService)
        {
            _db = dbContext;
            _protector = protector;
            _tokenService = tokenService;
        }

        // GET: api/user/all-users
        [HttpGet("all-users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserViewDto>> Get()
        {
            var allUsers = await _db.Users.ToListAsync();
            if (allUsers.Count == 0)
            {
                return NoContent();
            }
            var users = allUsers.Select(UserToDto).ToList();
            return Ok(users);
        }


        // GET: api/user/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserViewDto))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserViewDto>> GetUser(string id)
        {
            User user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exists");
            }

            return UserToDto(user);
        }

        // POST: api/user/create-user
        [HttpPost("create-user")]
        [ProducesResponseType(201, Type = typeof(GoogleBasicUserProfile))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] GoogleBasicUserProfile user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null)
            {
                return BadRequest("Request payload content is invalid");
            }
            
            var (hashedPassword, passwordSalt) = _protector.CreateHashedStringAndSalt(
                user.Password);
            var (hashedSecret, secretSalt) = _protector.CreateHashedStringAndSalt(
                user.Secret);
            
            user.Password = hashedPassword;
            user.Secret = hashedSecret;
            
            // passing `Salt` values as a parameter, because cannot set them
            // for a `GoogleBasicUserProfile`
            var newUser = GoogleUserToDatabaseDto(user, passwordSalt, secretSalt);

            await _db.Users.AddAsync(newUser);
            
            var affected = await _db.SaveChangesAsync();
            
            return affected == 1 ? 
                Created(nameof(GetUser), UserToDto(newUser)) : 
                Problem("Problem at the server, could not delete user");
        }

        [HttpPut("{id}/update-user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] GoogleBasicUserProfile userProvided,
            [FromQuery] string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userStored = await _db.Users.FindAsync(id);
            if (userStored == null)
            {
                return NotFound("User does not exist");
            }

            var isTokenValid = _tokenService.ValidateToken(token, userStored.UserSecretHash);
            if (!isTokenValid)
            {
                return Unauthorized("User is not verified");
            }
            
            var (hashedPassword, passwordSalt) = _protector.CreateHashedStringAndSalt(
                userProvided.Password);
            var (hashedSecret, secretSalt) = _protector.CreateHashedStringAndSalt(
                userProvided.Secret);
            
            // updating the stored user's information
            userStored.Username = userProvided.Username;
            userStored.UserEmail = userProvided.Email;
            userStored.UserPasswordHash = hashedPassword;
            userStored.UserPasswordSalt = passwordSalt;
            userStored.UserSecretHash = hashedSecret;
            userStored.UserPasswordSalt = secretSalt;
            userStored.Name = userProvided.Name;
            userStored.Gender = userProvided.Gender;
            userStored.Location = userProvided.Location;

            _db.Users.Update(userStored);
            int affected = await _db.SaveChangesAsync();
            if (affected == 1) // if one entry affected
            {
                return Ok("User Updated");
            }

            return Problem("Problem at server, could not update user");
        }

        [HttpDelete("{id}/delete-user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string id, [FromQuery] string token)
        {
            User user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exists");
            }
            
            var isTokenValid = _tokenService.ValidateToken(token, user.UserSecretHash);
            if (!isTokenValid)
            {
                return Unauthorized("User is not verified");
            }

            _db.Users.Remove(user);
            var affected = await _db.SaveChangesAsync();
            if (affected == 1)
            {
                return Ok("User Deleted");
            }
            return Problem("Problem at server, could not delete user");
        }

        private static UserViewDto UserToDto(User user) =>
            new UserViewDto
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                Username = user.Username,
                Confirmed = user.UserConfirmed,
                Name = user.Name,
                Gender = user.Gender,
                MemberSince = user.MemberSince
            };

        private static User UserToDatabaseDto(User user) =>
            new User
            {
                UserId = user.UserId,
                Username = user.Username,
                UserEmail = user.UserEmail,
                UserPasswordSalt = user.UserPasswordSalt,
                UserPasswordHash = user.UserPasswordHash,
                UserSecretHash = user.UserSecretHash,
                UserConfirmed = user.UserConfirmed,
                Name = user.Name,
                Location = user.Location,
                Gender = user.Gender,
                MemberSince = user.MemberSince,
                Passwords = user.Passwords
            };

        private User GoogleUserToDatabaseDto(GoogleBasicUserProfile user,
            string passwordSalt, string secretSalt)
        {
            // creating a new user with details from front-end
            var newUser = new User
            {
                Username = user.Username,
                UserEmail = user.Email,
                UserPasswordSalt = passwordSalt,
                UserPasswordHash = user.Password,
                UserSecretSalt = secretSalt,
                UserSecretHash = user.Secret,
                UserConfirmed = true,
                Name = user.Name,
                Location = user.Location,
                Gender = user.Gender,
                MemberSince = DateTime.Today.ToShortDateString()
            };
            
            // creating a unique uuid for user
            var uuid = _protector.GetUuid();
            newUser.UserId = uuid;

            return newUser;
        }
        
    }
}