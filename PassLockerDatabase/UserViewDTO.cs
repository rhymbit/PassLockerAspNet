using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassLockerDatabase
{
    public class UserViewDTO
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
