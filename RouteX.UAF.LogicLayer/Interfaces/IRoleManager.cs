using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Interfaces
{
    public interface IRoleManager
    {
        Task<APIResponse<List<RoleDto>>> GetAllRolesAsync();
        Task<APIResponse<RoleDto>> GetRoleByIdAsync(int id);
        Task<APIResponse<RoleDto>> AddRoleAsync(RoleDto dto, int loggedInUserId);
        Task<APIResponse<RoleDto>> UpdateRoleAsync(RoleDto dto, int loggedInUserId);
        Task<APIResponse<bool>> DeleteRoleAsync(int id, int loggedInUserId);
    }
}