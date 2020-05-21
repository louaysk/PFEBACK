using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Dto
{
    public class UserAddDto
    {

        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> role { get; set; }

       

        public UserAddDto(string id ,string f,string l, string e, string p, string u,List<string> role)
        {
            this.Id = id;
            this.Firstname = f;
            this.Lastname = l;
            this.Email = e;
            this.PhoneNumber = p;
            this.UserName = u;
            this.role = role;
        }

    }
}
