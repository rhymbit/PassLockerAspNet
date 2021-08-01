using Google.Apis.Auth;

namespace PassLocker.Services.GoogleLogin
{
    public class TestingPayload
    {
        private static GoogleJsonWebSignature.Payload testingPayload = new();

        public static GoogleJsonWebSignature.Payload GetTestingPayload()
        {
            testingPayload.Email = "prateekn332@gmail.com";
            testingPayload.Name = "Prateek Parashar";

            return testingPayload;
        }
    }
}