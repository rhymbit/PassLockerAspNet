using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using PassLocker.Dto;
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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GoogleLogin([FromBody] string googleToken)
        {
            GoogleJsonWebSignature.Payload payload =
                await _googleLogin.VerifyTokenAndGetPayload(googleToken);
            // Malicious User or Not an actual google user
            if (payload == null) return Unauthorized("Google user not verified");
            
            // Get user from the database or get a new user
            UserViewDto googleUser = _database.GetGoogleUser(payload.Email);
            googleUser.Name = payload.Name;
            googleUser.UserEmail = payload.Email;

            // Send basic user data
            // for a new user,
            // you have to create a 'password', 'secret answer' field at the front-end
            // these two can't be send out everytime user logs in
            return Ok(googleUser);
        }
    }
}
