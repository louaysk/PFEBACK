using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ApplicationSettings
    {
        public string JWT_Secret { get; set; }
        public string Client_URL { get; set; }
        public string Crayon_Client_Id { get; set; }
        public string Crayon_Client_Secret { get; set; }
        public string Crayon_Username { get; set; }
        public string Crayon_Password { get; set; }
    }
}
