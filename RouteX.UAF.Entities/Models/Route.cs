using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class Route : AuditableEntity
    {
        public int Id { get; set; }
        public string RouteName { get; set; } // E.g., "Satiana Road Express"

        public int RouteDirectionId { get; set; }
        public virtual RouteDirection Direction { get; set; }

        public virtual ICollection<BusStop> BusStops { get; set; }

        public Route()
        {
            BusStops = new HashSet<BusStop>();
        }
    }
}
