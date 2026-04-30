using RouteX.UAF.DAL;
using RouteX.UAF.Entities.Base;
using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.Entities.Models;
using RouteX.UAF.LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace RouteX.UAF.LogicLayer.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly ApplicationDbContext _context;

        public RoleManager()
        {
            _context = new ApplicationDbContext();
        }

        #region CRUD Operations

        public async Task<APIResponse<List<RoleDto>>> GetAllRolesAsync()
        {
            var response = new APIResponse<List<RoleDto>>();

            try
            {
                var roles = await _context.Roles.Where(r => r.IsActive).ToListAsync();

                if (roles == null || roles.Count == 0)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Roles not found.";
                    return response;
                }

                response.Data = roles.Select(MapEntityToDto).ToList();
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Roles retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}";
            }

            return response;
        }

        public async Task<APIResponse<RoleDto>> GetRoleByIdAsync(int id)
        {
            var response = new APIResponse<RoleDto>();

            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

                if (role == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Role not found.";
                    return response;
                }

                response.Data = MapEntityToDto(role);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Role retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}";
            }

            return response;
        }

        public async Task<APIResponse<RoleDto>> AddRoleAsync(RoleDto dto, int loggedInUserId)
        {
            var response = new APIResponse<RoleDto>();

            try
            {
                var role = new Role();
                role = MapDtoToEntity(dto, role, loggedInUserId);
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                dto.Id = role.Id; // Update DTO with the generated ID
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Role added successfully.";
                response.Data = dto;
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}";
            }

            return response;
        }

        public async Task<APIResponse<RoleDto>> UpdateRoleAsync(RoleDto dto, int loggedInUserId)
        {
            var response = new APIResponse<RoleDto>();

            try
            {
                var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.Id && r.IsActive);
                if (existingRole == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Role not found.";
                    return response;
                }

                var mappedRole = MapDtoToEntity(dto, existingRole, loggedInUserId);

                await _context.SaveChangesAsync();

                response.Data = MapEntityToDto(existingRole);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Role updated successfully.";
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = $"Exception: {ex.Message}";
            }

            return response;
        }

        public async Task<APIResponse<bool>> DeleteRoleAsync(int id, int loggedInUserId)
        {
            var response = new APIResponse<bool>();

            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

                if (role == null || !role.IsActive)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Role not found.";
                    response.Data = false;
                    return response;
                }

                // Soft delete the role
                role.IsActive = false;
                role.UpdatedAt = DateTime.Now;
                role.UpdatedBy = loggedInUserId;

                await _context.SaveChangesAsync();

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Role deleted successfully.";
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

        #region Helper Methods

        // Entity to DTO mapping
        private RoleDto MapEntityToDto(Role role)
        {
            if (role == null) return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        // DTO to Entity mapping
        private Role MapDtoToEntity(RoleDto dto, Role entity, int loggedInUserId)
        {
            if (dto == null) return null;

            entity.Id = dto.Id;
            entity.Name = dto.Name;
            entity.IsActive = true;

            // If Id is 0, it's a new record, so set creation properties
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

        #endregion Helper Methods
    }
}