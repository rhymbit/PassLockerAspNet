using System;
using System.Numerics;

namespace PassLocker.Dto
{
    public class UserViewDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        
        public string UserEmail { get; set; }
        public bool Confirmed { get; set; }
        public string Name { get; set; }
        
        public string Location { get; set; }
        public string Gender { get; set; }
        public string MemberSince { get; set; }
    }
}