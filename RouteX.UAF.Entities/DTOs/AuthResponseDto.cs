using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime LoginTime { get; set; } 
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string BaseUrl { get; set; }
        public string SessionId { get; set; }
    }
}
