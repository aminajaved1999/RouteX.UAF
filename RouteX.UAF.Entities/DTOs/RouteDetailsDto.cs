using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.DTOs
{
    public class RouteDetailsDto
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public string Direction { get; set; }
        public List<BusStopDto> Stops { get; set; }

        public RouteDetailsDto()
        {
            Stops = new List<BusStopDto>();
        }
    }
}
