using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.LogicLayer.Interfaces;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace RouteX.UAF.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : BaseApiController
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        #region CRUD Operations

        [HttpGet]
        [Route("GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var response = await _userManager.GetAllUsersAsync();
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IHttpActionResult> GetById(int? id)
        {
            // Validate that the 'id' is provided and is valid (greater than zero)
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("Please provide a valid User ID.");
            }

            var response = await _userManager.GetUserByIdAsync(id.Value);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IHttpActionResult> Add([FromBody] UserDto dto)
        {
            // Validate that the DTO is not null and contains all required fields
            if (dto == null)
            {
                return BadRequest("User data cannot be null.");
            }

            // Ensure required fields are provided in the DTO
            if (string.IsNullOrEmpty(dto.FullName))
            {
                return BadRequest("Full Name is required.");
            }
            
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest("Email is required.");
            }
            if (dto.RoleId <= 0)
            {
                return BadRequest("Role is required.");
            }

            var response = await _userManager.AddUserAsync(dto, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] UserDto dto)
        {
            // Validate that the DTO is not null and contains all required fields
            if (dto == null)
            {
                return BadRequest("User data cannot be null.");
            }

            // Ensure valid User ID is provided
            if (dto.Id <= 0)
            {
                return BadRequest("A valid User ID is required to update a record.");
            }

            if (string.IsNullOrEmpty(dto.FullName))
            {
                return BadRequest("Full Name is required.");
            }
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest("Email is required.");
            }
            if (dto.RoleId <= 0)
            {
                return BadRequest("Role is required.");
            }

            var response = await _userManager.UpdateUserAsync(dto, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IHttpActionResult> Delete(int? id)
        {
            // Validate that the 'id' is provided and is valid (greater than zero)
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("Please provide a valid User ID to delete.");
            }

            var response = await _userManager.DeleteUserAsync(id.Value, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        #endregion CRUD Operations
    }
}