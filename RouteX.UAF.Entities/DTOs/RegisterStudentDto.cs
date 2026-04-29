using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class RegisterStudentDto
    {
        public string FullName { get; set; }
        public string UafRegistrationNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
