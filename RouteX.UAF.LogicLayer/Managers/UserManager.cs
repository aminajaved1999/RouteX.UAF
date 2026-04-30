using RouteX.UAF.DAL;
using RouteX.UAF.Entities.Base;
using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.Entities.Models;
using RouteX.UAF.LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _context;
        public static readonly string UafRegistrationNumberRegex = @"^\d{4}-ag-\d{4}$";
        public static readonly string EmailRegex = @"^[a-zA-Z0-9._%+-]+@uaf\.edu\.pk$";
        public UserManager()
        {
            _context = new ApplicationDbContext();
        }

        #region CRUD Operations

        public async Task<APIResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var response = new APIResponse<List<UserDto>>();

            try
            {
                var users = await _context.Users.Where(u => u.IsActive).ToListAsync();

                if (users == null || users.Count == 0)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Users not found.";
                    return response;
                }

                response.Data = users.Select(MapEntityToDto).ToList();
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Users retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}"
                          + (ex.InnerException != null ? $"\n\nInner Exception: {ex.InnerException.Message}"
                          + (ex.InnerException.InnerException != null ? $"\n\nInner Inner Exception: {ex.InnerException.InnerException.Message}" : "") : "");

            }

            return response;
        }

        public async Task<APIResponse<UserDto>> GetUserByIdAsync(int id)
        {
            var response = new APIResponse<UserDto>();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

                if (user == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                response.Data = MapEntityToDto(user);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "User retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}"
                          + (ex.InnerException != null ? $"\n\nInner Exception: {ex.InnerException.Message}"
                          + (ex.InnerException.InnerException != null ? $"\n\nInner Inner Exception: {ex.InnerException.InnerException.Message}" : "") : "");

            }

            return response;
        }

        public async Task<APIResponse<UserDto>> AddUserAsync(UserDto dto, int loggedInUserId)
        {
            var response = new APIResponse<UserDto>();

            try
            {
                var user = new User();
                user = MapDtoToEntity(dto, user, loggedInUserId);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password); // Hash the password here
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                dto.Id = user.Id; // Update DTO with the generated ID
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "User added successfully.";
                response.Data = dto;
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}"
                          + (ex.InnerException != null ? $"\n\nInner Exception: {ex.InnerException.Message}"
                          + (ex.InnerException.InnerException != null ? $"\n\nInner Inner Exception: {ex.InnerException.InnerException.Message}" : "") : "");

            }

            return response;
        }

        public async Task<APIResponse<UserDto>> UpdateUserAsync(UserDto dto, int loggedInUserId)
        {
            var response = new APIResponse<UserDto>();

            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Id == dto.Id && u.IsActive);
                if (existingUser == null || !existingUser.IsActive)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                // Run validations
                var validation = await UserValidationCheckAsync(dto);
                if (!validation.isSuccess)
                {
                    response.Code = validation.Code;
                    response.Message = validation.Message;
                    return response;
                }

                // Check if password is provided in the request to be updated
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    // Hash the new password if it's provided
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }


                var mappingUser = MapDtoToEntity(dto, existingUser, loggedInUserId);

                await _context.SaveChangesAsync();

                response.Data = MapEntityToDto(existingUser);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "User updated successfully.";
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

        public async Task<APIResponse<bool>> DeleteUserAsync(int id, int loggedInUserId)
        {
            var response = new APIResponse<bool>();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null || !user.IsActive)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    response.Data = false;
                    return response;
                }

                // Soft delete
                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = loggedInUserId;

                await _context.SaveChangesAsync();

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "User deleted successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}";
            }

            return response;
        }

        #endregion CRUD Operations

        #region Helper Mapping Methods

        private UserDto MapEntityToDto(User user)
        {
            if (user == null) return null;

            var userDto = new UserDto();
            userDto.Id = user.Id;
            userDto.FullName = user.FullName;
            userDto.UafRegistrationNumber = user.UafRegistrationNumber;
            userDto.Email = user.Email;
            userDto.RoleDto = new RoleDto();
            userDto.RoleDto.Id = user.Role.Id;
            userDto.RoleDto.Name = user.Role.Name;

            return userDto;
        }
        private User MapDtoToEntity(UserDto dto, User entity, int loggedInUserId)
        {
            if (dto == null) return null;

            entity.Id = dto.Id;
            entity.FullName = dto.FullName;
            entity.UafRegistrationNumber = dto.UafRegistrationNumber;
            entity.Email = dto.Email;
            entity.IsActive = true;

            if (dto.RoleId > 0)
            {
                entity.RoleId = dto.RoleId;
            }

            if (dto.Id == 0)
            {
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = loggedInUserId;
            }
            else
            {
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = loggedInUserId;
            }

            return entity;
        }

        #endregion Helper Mapping Methods

        #region Validation Methods

        private async Task<APIResponse<string>> UserValidationCheckAsync(UserDto dto)
        {
            var res = new APIResponse<string>();
           

            // 1. Validate RoleId (check if role exists in the database)
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId);
            if (role == null)
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "The specified role does not exist.";
                return res;
            }

            // 2. Validate Role-specific conditions
            if (role.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                // If role is "Student", validate that email ends with @uaf.edu.pk
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Email, EmailRegex))
                {
                    res.isSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid email format for Student. A valid email should look like: username@uaf.edu.pk.";
                    return res;
                }

                // Validate UafRegistrationNumber is not empty for Student role
                if (string.IsNullOrEmpty(dto.UafRegistrationNumber))
                {
                    res.isSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "UAF Registration Number is required for Student.";
                    return res;
                }

                // Validate UafRegistrationNumber format using Regex
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.UafRegistrationNumber, UafRegistrationNumberRegex))
                {
                    res.isSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid UAF Registration Number format. It should follow the format: YYYY-ag-1234 (e.g., 2022-ag-1234).";
                    return res;
                }

                // Validate UafRegistrationNumber format for Student role
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.UafRegistrationNumber, UafRegistrationNumberRegex))
                {
                    res.isSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid UAF Registration Number format for Student. It should follow the format: YYYY-ag-1234.";
                    return res;
                }
            }
            else if (role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                // If role is "Admin", allow any email format and no registration number validation
                // No need for validation here
            }
            else
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid role specified.";
                return res;
            }

            // 4. Validate Email (check for duplicate)
            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != dto.Id && u.IsActive);
            if (emailExists)
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "A user with this email already exists.";
                return res;
            }

            // 5. Validate UafRegistrationNumber (check for duplicate)
            var registrationNumberExists = await _context.Users.AnyAsync(u => u.UafRegistrationNumber == dto.UafRegistrationNumber && u.Id != dto.Id && u.IsActive);
            if (registrationNumberExists)
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "A user with this registration number already exists.";
                return res;
            }

            // All validations passed
            res.isSuccess = true;
            res.Code = (int)HttpStatusCode.OK;
            res.Message = "Validation successful";
            return res;
        }
        #endregion Validation Methods
    }
}