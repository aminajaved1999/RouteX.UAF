using BCrypt.Net;
using RouteX.UAF.DAL;
using RouteX.UAF.Entities.Base;
using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.Entities.Enums;
using RouteX.UAF.Entities.Models;
using RouteX.UAF.LogicLayer.Interfaces;
using RouteX.UAF.LogicLayer.Services;
using RouteX.UAF.LogicLayer.Utilities;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AuthManager()
        {
            _context = new ApplicationDbContext();
            _emailService = new EmailService();
        }


        public APIResponse<AuthResponseDto> Login(LoginDto dto)
        {
            var response = new APIResponse<AuthResponseDto>();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");
                    var studentRole = _context.Roles.FirstOrDefault(r => r.Name == "Student");

                    if (adminRole == null || studentRole == null)
                    {
                        response.Code = (int)HttpStatusCode.InternalServerError;
                        response.Message = "Roles not found in the database.";
                        return response;
                    }

                    var user = _context.Users.FirstOrDefault(u => u.IsActive &&
                        ((u.RoleId == adminRole.Id && u.Email == dto.UafRegistrationNumber) ||
                        (u.RoleId == studentRole.Id && (u.UafRegistrationNumber != null && u.UafRegistrationNumber == dto.UafRegistrationNumber) || u.Email == dto.Email)));

                    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    {
                        response.Code = (int)HttpStatusCode.Unauthorized;
                        response.Message = "Invalid credentials.";
                        return response;
                    }

                    if (user.RoleId == studentRole.Id && !user.IsEmailVerified)
                    {
                        response.Code = (int)HttpStatusCode.Forbidden;
                        response.Message = "Your email is not verified.";
                        return response;
                    }

                    string roleName = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;
                    string jwtToken = JwtTokenGenerator.GenerateToken(user.Id, user.FullName, roleName);
                    string refreshToken = JwtTokenGenerator.GenerateRefreshToken();
                    string sessionId = Guid.NewGuid().ToString();
                    DateTime loginTime = DateTime.Now; // Capture once for consistency

                    CleanupExpiredRefreshTokens(user.Id);

                    _context.RefreshTokens.Add(new RefreshToken
                    {
                        UserId = user.Id,
                        Token = refreshToken,
                        ExpiryDate = DateTime.Now.AddDays(7),
                        IsRevoked = false,
                        SessionId = sessionId,
                        UserAgent = dto.UserAgent
                    });

                    _context.DeviceTokens.Add(new DeviceToken
                    {
                        UserId = user.Id,
                        Token = dto.DeviceToken,
                        SessionId = sessionId,
                        CreatedAt = loginTime,
                    });

                    _context.LoginLogs.Add(new LoginLog
                    {
                        UserId = user.Id,
                        LoginTime = loginTime,
                        IsLoggedIn = true,
                        AccessToken = jwtToken,
                        BaseUrl = dto.CurrentBaseUrl,
                        IpAddress = dto.ipAddress,
                        UserAgent = dto.UserAgent
                    });

                    _context.SaveChanges();
                    transaction.Commit();

                    response.Code = (int)HttpStatusCode.OK;
                    response.Message = "Login successful.";

                    // Updated Response Mapping
                    response.Data = new AuthResponseDto
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        RoleId = user.RoleId,          
                        Role = roleName,
                        IsEmailVerified = user.IsEmailVerified,
                        LoginTime = loginTime,        
                        Token = jwtToken,
                        RefreshToken = refreshToken,
                        BaseUrl = dto.CurrentBaseUrl,   
                        SessionId = sessionId
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.Code = (int)HttpStatusCode.InternalServerError;
                    response.Message = $"Exception: {ex.Message}";
                }
            }

            return response;
        }

        public APIResponse<bool> LogoutFromAllDevices(string accessToken)
        {
            var response = new APIResponse<bool>();

            try
            {
                var loginLog = _context.LoginLogs.FirstOrDefault(l => l.AccessToken == accessToken && l.IsLoggedIn);
                if (loginLog == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Active session not found.";
                    return response;
                }

                var userId = loginLog.UserId;

                // Invalidate all refresh tokens
                var userRefreshTokens = _context.RefreshTokens.Where(r => r.UserId == userId && !r.IsRevoked).ToList();
                foreach (var refreshToken in userRefreshTokens)
                {
                    refreshToken.IsRevoked = true;
                }

                // Deactivate all device tokens
                var userDeviceTokens = _context.DeviceTokens.Where(t => t.UserId == userId).ToList();
                foreach (var deviceToken in userDeviceTokens)
                {
                    deviceToken.IsActive = false;
                }

                // Mark the current login log as logged out
                loginLog.IsLoggedIn = false;
                loginLog.LogoutTime = DateTime.Now;

                _context.SaveChanges();

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Logged out from all devices successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Error occurred during logout from all devices: {ex.Message}";
            }

            return response;
        }

        public APIResponse<bool> LogoutFromCurrentDevice(string accessToken, string deviceToken)
        {
            var response = new APIResponse<bool>();

            try
            {
                var loginLog = _context.LoginLogs.FirstOrDefault(l => l.AccessToken == accessToken && l.IsLoggedIn);
                if (loginLog == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Active session not found.";
                    return response;
                }

                // Find and deactivate the device token
                var deviceEntry = _context.DeviceTokens.FirstOrDefault(t => t.Token == deviceToken);
                if (deviceEntry == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Device token not found.";
                    return response;
                }

                deviceEntry.IsActive = false;  // Deactivate the current device token

                // Mark the current login log as logged out
                loginLog.IsLoggedIn = false;
                loginLog.LogoutTime = DateTime.Now;

                _context.SaveChanges();

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Logged out from the current device successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Error occurred during logout from current device: {ex.Message}";
            }

            return response;
        }

        private void CleanupExpiredRefreshTokens(int userId)
        {
            var expiredTokens = _context.RefreshTokens.Where(r => r.UserId == userId && r.ExpiryDate < DateTime.Now).ToList();
            foreach (var token in expiredTokens)
            {
                _context.RefreshTokens.Remove(token);
            }
            _context.SaveChanges();
        }

      


        public async Task<APIResponse<bool>> RegisterStudentAsync(RegisterStudentDto dto)
        {
            var response = new APIResponse<bool>();

            try
            {
                if (!dto.Email.EndsWith("@uaf.edu.pk", StringComparison.OrdinalIgnoreCase))
                {
                    response.Code = (int)HttpStatusCode.BadRequest;
                    response.Message = "A valid @uaf.edu.pk email is required.";
                    return response;
                }

                if (_context.Users.Any(u => u.Email == dto.Email || u.UafRegistrationNumber == dto.UafRegistrationNumber))
                {
                    response.Code = (int)HttpStatusCode.Conflict;
                    response.Message = "Student with this email or registration number already exists.";
                    return response;
                }

                string otp = new Random().Next(100000, 999999).ToString();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var newUser = new User
                {
                    FullName = dto.FullName,
                    UafRegistrationNumber = dto.UafRegistrationNumber,
                    Email = dto.Email,
                    PasswordHash = hashedPassword,
                    RoleId = (int)RoleType.Student,
                    IsEmailVerified = false,
                    CurrentOtp = otp,
                    OtpExpiry = DateTime.UtcNow.AddMinutes(10)
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                await _emailService.SendOtpEmailAsync(dto.Email, otp, dto.FullName);

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Registration successful. Please check your email for the OTP.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = response.Message = $"Exception: {ex.Message}"
                         + (ex.InnerException != null ? $"\n\nInner Exception: {ex.InnerException.Message}"
                         + (ex.InnerException.InnerException != null ? $"\n\nInner Inner Exception: {ex.InnerException.InnerException.Message}" : "") : "");

            }

            return response;
        }

        public APIResponse<AuthResponseDto> VerifyOtp(VerifyOtpDto dto)
        {
            var response = new APIResponse<AuthResponseDto>();

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

                if (user == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                if (user.IsEmailVerified)
                {
                    response.Code = (int)HttpStatusCode.BadRequest;
                    response.Message = "Email is already verified.";
                    return response;
                }

                if (user.CurrentOtp != dto.OtpCode || DateTime.UtcNow > user.OtpExpiry)
                {
                    response.Code = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Invalid or expired OTP.";
                    return response;
                }

                user.IsEmailVerified = true;
                user.CurrentOtp = null;
                user.OtpExpiry = null;
                _context.SaveChanges();

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Email verified successfully.";
                response.Data = new AuthResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Role = "Student",
                    IsEmailVerified = true
                };
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = response.Message = $"Exception: {ex.Message}"
                         + (ex.InnerException != null ? $"\n\nInner Exception: {ex.InnerException.Message}"
                         + (ex.InnerException.InnerException != null ? $"\n\nInner Inner Exception: {ex.InnerException.InnerException.Message}" : "") : "");

            }

            return response;
        }
    }
}