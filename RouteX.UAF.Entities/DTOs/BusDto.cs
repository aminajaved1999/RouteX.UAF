using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class BusDto
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }
        public bool IsCurrentlyActive { get; set; }
        public string DriverName { get; set; } // Nullable if unassigned
    }
}
