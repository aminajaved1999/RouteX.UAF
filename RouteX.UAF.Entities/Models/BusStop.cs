using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class BusStop : AuditableEntity
    {
        public int Id { get; set; }
        public string StopName { get; set; } // E.g., "McDonalds Satiana Rd"

        // Spatial Coordinates
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Sequence order to draw the line on Google Maps
        public int StopOrder { get; set; }

        // Tells the app if students can board here, get off here, or both
        public int StopActionId { get; set; }
        public virtual StopAction Action { get; set; }

        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
    }
}
