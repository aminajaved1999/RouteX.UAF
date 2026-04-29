using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class Role : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // E.g., "Admin", "Driver", "Student"

        public virtual ICollection<User> Users { get; set; }
    }
}
