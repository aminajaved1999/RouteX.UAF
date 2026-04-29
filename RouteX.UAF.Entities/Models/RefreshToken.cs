using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class RefreshToken 
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public string Token { get; set; } // The secure, randomly generated refresh token string
        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; } // Allows you to forcefully log a user out from the admin panel
        public string SessionId { get; set; }
        public string UserAgent { get; set; } // Logs if they logged in via "Android App" or "iOS App"
    }
}
