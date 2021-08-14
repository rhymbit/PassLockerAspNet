using Microsoft.AspNetCore.Mvc;

namespace PassLocker.Dto
{
    public class Tokens
    {
        [FromBody]
        public string GoogleToken { get; set; }
        
        [FromBody]
        public string PasswordToken { get; set; }
    }
}