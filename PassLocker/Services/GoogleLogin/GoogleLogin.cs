using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using PassLocker.Controllers;

namespace PassLocker.Services.GoogleLogin
{
    public class GoogleLogin : IGoogleLogin
    {
        private IConfiguration Configs { get; }

        public GoogleLogin(IConfiguration configuration)
        {
            Configs = configuration;
        }

        private readonly string[] GoogleIssuer =
        {
            "accounts.google.com",
            "https://accounts.google.com"
        };
        
        public async Task<GoogleJsonWebSignature.Payload> VerifyTokenAndGetPayload(Token token)
        {
            GoogleJsonWebSignature.Payload payload = null;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(token.googleToken);

                if (!payload.Audience.Equals(Configs["GoogleAuth:ClientId"]))
                {
                    Console.WriteLine("Not a valid user.");
                }
                
                if (!payload.Issuer.Equals(GoogleIssuer[0]) &&
                    !payload.Issuer.Equals(GoogleIssuer[1]))
                {
                    Console.WriteLine("Not a valid user");
                }
                // ****** Code to check token expiration *******
                // DateTime now = DateTime.Now.ToUniversalTime();
                // DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds)
                //     .DateTime;
                // if (now > expiration)
                // {
                //     Console.WriteLine("Not a valid user");
                //     return BadRequest("Invalid User");
                // }
            }
            catch (InvalidJwtException exp)
            {
                Console.WriteLine("Invalid token from Google User.");
            }
            
            // if all checks pass
            return payload;
        }
    }
}