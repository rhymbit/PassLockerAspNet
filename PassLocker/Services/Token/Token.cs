using Microsoft.AspNetCore.Mvc;

namespace PassLocker.Services.Token
{
    public class Token
    {
        [FromBody]
        public string GoogleToken { get; set; }
        
        [FromBody]
        public string PasswordToken { get; set; }
    }
}