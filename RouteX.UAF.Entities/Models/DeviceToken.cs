using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class DeviceToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public string Token { get; set; } // The FCM or APNs device token
        public string SessionId { get; set; }
    }
}
