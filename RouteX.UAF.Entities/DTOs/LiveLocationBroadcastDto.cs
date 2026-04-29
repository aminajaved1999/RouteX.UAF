using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class LiveLocationBroadcastDto
    {
        public int BusId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LicensePlate { get; set; }
        // Optionally include an ETA property if your backend calculates it before broadcasting
        public int EstimatedMinutesToNextStop { get; set; }
    }
}
