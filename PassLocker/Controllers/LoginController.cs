using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using PassLocker.Database;
using PassLocker.Services.GoogleLogin;
using PassLocker.Services.Token;
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
            
            // Malicious User or Not an actual google user
            if (payload.Equals(null)) return BadRequest("Google user not verified.");
            
            // Get user from the database or get a new user
            UserViewDTO googleUser = await _database.GetGoogleUser(payload.Email);
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
