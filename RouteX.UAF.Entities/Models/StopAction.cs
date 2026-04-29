using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class StopAction : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // E.g., "PickupOnly", "DropoffOnly", "Both"

        public virtual ICollection<BusStop> BusStops { get; set; }
    }
}
