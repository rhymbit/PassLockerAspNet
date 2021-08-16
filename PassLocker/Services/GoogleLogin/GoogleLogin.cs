using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace PassLocker.Services.GoogleLogin
{
    public class GoogleLogin : IGoogleLogin
    {
        private IConfiguration Configs { get; }

        public GoogleLogin(IConfiguration configuration)
        {
            Configs = configuration;
        }

        private readonly string[] _googleIssuer =
        {
            "accounts.google.com",
            "https://accounts.google.com"
        };
        
        public async Task<GoogleJsonWebSignature.Payload> VerifyTokenAndGetPayload(string token)
        {
            GoogleJsonWebSignature.Payload payload = null;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(token);

                if (payload != null && !payload.Audience.Equals(Configs["GoogleAuth:ClientId"]))
                {
                    // Console.WriteLine("PassLocker.Services.GoogleLogin - Not a valid user.");
                    payload = null;
                }
                
                if (payload != null && !payload.Issuer.Equals(_googleIssuer[0]) && !payload.Issuer.Equals(_googleIssuer[1]))
                {
                    // Console.WriteLine("PassLocker.Services.GoogleLogin - Not a valid user");
                    payload = null;
                }
                // ****** Code to check token expiration *******
                DateTime now = DateTime.Now.ToUniversalTime();
                if (payload?.ExpirationTimeSeconds != null)
                {
                    var expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds)
                        .DateTime;
                    if (now > expiration)
                    {
                        // Console.WriteLine("Not a valid user");
                        payload = null;
                    }
                }
            }
            catch (InvalidJwtException)
            {
                Console.WriteLine("PassLocker.Services.GoogleLogin - Invalid token from Google User.");
                payload = null;
            }

            return payload;
        }
    }
}