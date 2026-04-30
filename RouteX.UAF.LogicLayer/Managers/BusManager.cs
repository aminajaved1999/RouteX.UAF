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
    public class BusManager : IBusManager
    {
        private readonly ApplicationDbContext _context;

        public BusManager()
        {
            _context = new ApplicationDbContext();
        }


        #region crud operations
        public async Task<APIResponse<List<BusDto>>> GetAllBusesAsync()
        {
            var response = new APIResponse<List<BusDto>>();

            try
            {
                var buses = await _context.Buses.Where(b => b.IsActive).ToListAsync();

                if (buses == null || buses.Count == 0)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Buses not found.";
                    return response;
                }

                response.Data = buses.Select(MapEntityToDto).ToList();
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Buses retrieved successfully.";
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
        public async Task<APIResponse<BusDto>> GetBusByIdAsync(int id)
        {
            var response = new APIResponse<BusDto>();

            try
            {
                var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == id && b.IsActive);

                if (bus == null)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Bus not found.";
                    return response;
                }

                response.Data = MapEntityToDto(bus);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Bus retrieved successfully.";
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
        public async Task<APIResponse<BusDto>> AddBusAsync(BusDto dto, int loggedInUserId)
        {
            var response = new APIResponse<BusDto>();

            try
            {
                var bus = new Bus();
                bus = MapDtoToEntity(dto, bus, loggedInUserId);
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();

                dto.Id = bus.Id; // Update DTO with the generated ID
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Bus added successfully.";
                response.Data = dto;
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
        public async Task<APIResponse<BusDto>> UpdateBusAsync(BusDto dto, int loggedInUserId)
        {
            var response = new APIResponse<BusDto>();

            try
            {
                // Run validations
                var validation = await BusValidationCheckAsync(dto);
                if (!validation.isSuccess)
                {
                    response.Code = validation.Code;
                    response.Message = validation.Message;
                    return response;
                }

                // Map and save
                var existingBus = _context.Buses.FirstOrDefault(b => b.Id == dto.Id && b.IsActive);
                var mappingBus = MapDtoToEntity(dto, existingBus, loggedInUserId);

                await _context.SaveChangesAsync();

                response.Data = MapEntityToDto(existingBus);
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Bus updated successfully.";
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
        public async Task<APIResponse<bool>> DeleteBusAsync(int id, int loggedInUserId)
        {
            var response = new APIResponse<bool>();

            try
            {
                var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == id);

                if (bus == null || !bus.IsActive)
                {
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Message = "Bus not found.";
                    response.Data = false;
                    return response;
                }

                // Performing a soft delete
                bus.IsActive = false;
                bus.UpdatedAt = DateTime.Now;
                bus.UpdatedBy = loggedInUserId;

                await _context.SaveChangesAsync();

                response.Code = (int)HttpStatusCode.OK;
                response.Message = "Bus deleted successfully.";
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
        #endregion crud operations

        #region Helper Mapping Methods

        // Entity to DTO
        private BusDto MapEntityToDto(Bus bus)
        {
            if (bus == null) return null;

            var busDto = new BusDto();
            busDto.Id = bus.Id;
            busDto.LicensePlate = bus.LicensePlate;
            busDto.Capacity = bus.Capacity;

            return busDto;
        }

        // DTO to Entity
        private Bus MapDtoToEntity(BusDto dto, Bus entity, int loggedInUserId)
        {
            if (dto == null) return null;

            entity.Id = dto.Id;
            entity.LicensePlate = dto.LicensePlate;
            entity.Capacity = dto.Capacity;

            // If Id is 0, it's a new record, so set creation properties
            if (dto.Id == 0)
            {
                entity.IsActive = true;
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = loggedInUserId;
            }
            // If Id is > 0, it's an update, so set update properties
            else
            {
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = loggedInUserId;
            }

            return entity;
        }

        #endregion Helper Mapping Methods

        #region Validation Methods 
        private async Task<APIResponse<string>> BusValidationCheckAsync(BusDto dto)
        {
            var res = new APIResponse<string>();

            // 1. Check capacity first
            if (dto.Capacity <= 0)
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "Capacity must be greater than zero.";
                return res;
            }

            // 2. Fetch the bus
            var existingBus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == dto.Id && b.IsActive);

            if (existingBus == null)
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.NotFound;
                res.Message = "Bus not found.";
                return res;
            }

            // 3. Check for duplicate license plates
            var isLicensePlateDuplicate = await _context.Buses
                .AnyAsync(b => b.LicensePlate == dto.LicensePlate
                            && b.Id != dto.Id
                            && b.IsActive);

            if (isLicensePlateDuplicate)
            {
                res.isSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "Another active bus with the same license plate already exists.";
                return res;
            }

            if (dto.DriverId > 0)
            {
                // 4. Check driver existence
                var driverExists = await _context.Users.AnyAsync(u => u.Id == dto.DriverId);

                if (!driverExists)
                {
                    res.isSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "Assigned driver does not exist.";
                    return res;
                }

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