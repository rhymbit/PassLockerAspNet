using System.Threading.Tasks;
using Google.Apis.Auth;
using PassLocker.Controllers;

namespace PassLocker.Services.GoogleLogin
{
    public interface IGoogleLogin
    {
        public Task<GoogleJsonWebSignature.Payload> VerifyTokenAndGetPayload(Token token);
    }
}