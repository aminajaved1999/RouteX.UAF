using RouteX.UAF.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Models
{
    public class Bus : AuditableEntity
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }

        public bool IsCurrentlyActive { get; set; }

        // Nullable because a bus might be parked or unassigned temporarily
        public int? DriverId { get; set; }
        public virtual User Driver { get; set; }

        public virtual ICollection<LocationLog> LocationLogs { get; set; }
    }
}
