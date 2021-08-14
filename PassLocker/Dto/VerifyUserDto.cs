using Microsoft.AspNetCore.Mvc;

namespace PassLocker.Dto
{
    public class VerifyUserDto
    {
        [FromBody] public string password { get; set; }

        [FromBody] public string secret { get; set; }
    }
}