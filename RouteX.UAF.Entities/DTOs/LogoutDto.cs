using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class LogoutDto
    {
        public int UserId { get; set; }
        public string SessionId { get; set; }
        public string RefreshToken { get; set; } // Used to identify which token to revoke
    }
}
