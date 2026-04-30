using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class LogoutDto
    {
        public string AccessToken { get; set; }  // JWT access token to identify the user
        public string DeviceToken { get; set; }  // Unique device token (e.g., FCM token)

    }
}
