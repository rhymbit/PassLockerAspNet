using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassLocker.Database
{
    public class UserViewDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool Confirmed { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Gender { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
