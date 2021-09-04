using Microsoft.AspNetCore.Mvc;

namespace PassLocker.Dto
{
    public class Tokens
    {
        // This is pretty much useless with the current frontend code
        [FromBody]
        public string GoogleToken { get; set; }
        
        // This is used by Password Controller to verify user's identity
        [FromBody]
        public string PasswordToken { get; set; }
        
        // This is used by User Controller to verify user's identity
        [FromBody]
        public string UserToken { get; set; }
    }
}