using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class LoginLog 
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; } // Nullable because they might still be logged in

        public bool IsLoggedIn { get; set; }

        public string AccessToken { get; set; } // Can store the JWT or a specific Session identifier
        public string BaseUrl { get; set; }
        public string IpAddress { get; set; } // Great addition for security monitoring
        public string UserAgent { get; set; } // Great addition for security monitoring
    }
}
