using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class TokenRefreshDto
    {
        public string Token { get; set; } // The expired JWT
        public string RefreshToken { get; set; } // The valid refresh token stored on the device
        public string DeviceToken { get; set; } // Keep the device token updated
    }
}
