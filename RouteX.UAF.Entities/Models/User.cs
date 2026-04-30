using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class User : AuditableEntity
    {
        public int Id { get; set; }

        // Basic Profile Info
        public string FullName { get; set; }
        public string UafRegistrationNumber { get; set; } // E.g., "2018-AG-1234"
        public string Email { get; set; } // Email for OTP verification or logging in
        public string PasswordHash { get; set; }

        // OTP Verification Fields
        public bool IsEmailVerified { get; set; }
        public string CurrentOtp { get; set; }
        public DateTime? OtpExpiry { get; set; }

        // Foreign Key to the Role lookup table
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        // Navigation Properties for Session & Security
        public virtual ICollection<DeviceToken> DeviceTokens { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<LoginLog> LoginLogs { get; set; }

        public User()
        {
            DeviceTokens = new HashSet<DeviceToken>();
            RefreshTokens = new HashSet<RefreshToken>();
            LoginLogs = new HashSet<LoginLog>();
        }


    }
}
