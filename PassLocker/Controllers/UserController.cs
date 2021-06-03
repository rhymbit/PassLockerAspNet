using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PassLocker.Database;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PassLocker.Controllers
{
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private PassLockerDbContext db;
        public UserController(PassLockerDbContext injectContext)
        {
            this.db = injectContext;
        }
        
        // GET: api/user
        [HttpGet]
        [ProducesResponseType(404)]
        public IActionResult Get()
        {
            return BadRequest("Inaccessible url");
        }

        // GET: api/user/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User does not exists");
            }
            return UserToDTO(user);
        }

        // POST: api/user/create-user
        [HttpPost("create-user")]
        [ProducesResponseType(201, Type = typeof(UserDTO))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid request content. User's info is invalid");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EntityEntry<User> added = await db.Users.AddAsync(user);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return CreatedAtRoute(
                    routeName: nameof(GetUser),
                    routeValues: new { id = user.UserId },
                    value: UserToDTO(user));
            }
            else
            {
                return Problem("Some problem at the server");
            }
        }

        [HttpPut("{id:int}/edit-profile")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || user.UserId != id)
            {
                return BadRequest("Invalid request content. User's info is invalid");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Update(user);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return NoContent();
            }
            else
            {
                return NotFound("User could not be found in database");
            }
        }

        [HttpDelete("{id:int}/delete-profile")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User does not exists");
            }
            db.Users.Remove(user);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return NoContent();
            }
            else
            {
                return NotFound("User could not be found in database");
            }
        }

        private static UserDTO UserToDTO(User user) =>
            new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Confirmed = user.Confirmed,
                Name = user.Name,
                Gender = user.Gender,
                MemberSince = user.MemberSince
            };

    }
}
