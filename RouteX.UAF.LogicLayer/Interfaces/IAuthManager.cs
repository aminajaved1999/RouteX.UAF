using RouteX.UAF.Entities.Base;
using RouteX.UAF.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Interfaces
{
    public interface IAuthManager
    {
        APIResponse<AuthResponseDto> Login(LoginDto dto);

        APIResponse<bool> LogoutFromCurrentDevice(string accessToken, string deviceToken);

        APIResponse<bool> LogoutFromAllDevices(string accessToken);

        Task<APIResponse<bool>> RegisterStudentAsync(RegisterStudentDto dto);
        APIResponse<AuthResponseDto> VerifyOtp(VerifyOtpDto dto);
    }
}
