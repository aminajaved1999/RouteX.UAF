using RouteX.UAF.DAL;
using RouteX.UAF.Entities.Base;
using RouteX.UAF.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Interfaces
{
    public interface IBusManager
    {
        Task<APIResponse<List<BusDto>>> GetAllBusesAsync();
        Task<APIResponse<BusDto>> GetBusByIdAsync(int id);
        Task<APIResponse<BusDto>> AddBusAsync(BusDto dto, int loggedInUserId);
        Task<APIResponse<BusDto>> UpdateBusAsync(BusDto dto, int loggedInUserId);
        Task<APIResponse<bool>> DeleteBusAsync(int id, int loggedInUserId);
    }
}
