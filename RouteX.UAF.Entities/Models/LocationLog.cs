using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class LocationLog : AuditableEntity
    {
        public long Id { get; set; } // 'long' because live tracking generates thousands of rows quickly

        public int BusId { get; set; }
        public virtual Bus Bus { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Speed { get; set; } // Crucial for calculating accurate ETAs

        public DateTime Timestamp { get; set; }
    }
}
