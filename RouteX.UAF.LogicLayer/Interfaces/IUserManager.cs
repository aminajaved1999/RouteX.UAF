using RouteX.UAF.Entities.Base;
using RouteX.UAF.Entities.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Interfaces
{
    public interface IUserManager
    {
        Task<APIResponse<List<UserDto>>> GetAllUsersAsync();
        Task<APIResponse<UserDto>> GetUserByIdAsync(int id);
        Task<APIResponse<UserDto>> AddUserAsync(UserDto dto, int loggedInUserId);
        Task<APIResponse<UserDto>> UpdateUserAsync(UserDto dto, int loggedInUserId);
        Task<APIResponse<bool>> DeleteUserAsync(int id, int loggedInUserId);
    }
}