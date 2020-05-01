using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Dto
{
    public class UserAddDto
    {

        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }

        public UserAddDto(string id ,string f, string e, string p, string u)
        {
            this.Id = id;
            this.FullName = f;
            this.Email = e;
            this.PhoneNumber = p;
            this.UserName = u;
        }

    }
}
