using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using PassLocker.Services.GoogleLogin;
using PassLocker.Services.UserDatabase;

namespace PassLocker.Controllers
{
    [Route("api/[Controller]")]
    public class LoginController : Controller
    {
        private readonly IGoogleLogin _googleLogin;
        private readonly IUserDatabase _database;
        public LoginController(IGoogleLogin googleLogin, IUserDatabase userDatabase)
        {
            _googleLogin = googleLogin;
            _database = userDatabase;
        }

        [HttpPost("google-login")]
        public async Task<ActionResult> GoogleLogin([FromBody] Token googleAuthToken)
        {
            GoogleJsonWebSignature.Payload payload =
                await _googleLogin.VerifyTokenAndGetPayload(googleAuthToken);
            
            if (payload.Equals(null)) return BadRequest("Google user not verified.");
            
            // check for user in the database
            if (await _database.CheckIfUserExists(payload.Email))
            {
                // return redirect to user's profile page
            }
            // if user does not exist redirects to new user page
            return Redirect("http://localhost:3000");

        }
    }
}
