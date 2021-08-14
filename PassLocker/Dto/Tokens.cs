using Microsoft.AspNetCore.Mvc;

namespace PassLocker.Dto
{
    public class Tokens
    {
        [FromBody]
        public string googleToken { get; set; }
        
        [FromBody]
        public string passwordToken { get; set; }
    }
}