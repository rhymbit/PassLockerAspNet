using System.Threading.Tasks;
using Google.Apis.Auth;

namespace PassLocker.Services.GoogleLogin
{
    public interface IGoogleLogin
    {
        public Task<GoogleJsonWebSignature.Payload> VerifyTokenAndGetPayload(string token);
    }
}