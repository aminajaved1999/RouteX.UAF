using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class LoginDto
    {
        public string UafRegistrationNumber { get; set; } // Can also use Email
        public string Password { get; set; }

        // New Additions for Session & Notifications
        public string DeviceToken { get; set; } // The FCM/APNs token from the phone
        public string UserAgent { get; set; } // E.g., "Android App v1.2" or "iOS App"
    }
}
