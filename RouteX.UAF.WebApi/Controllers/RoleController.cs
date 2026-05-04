using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.LogicLayer.Interfaces;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace RouteX.UAF.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Role")]
    public class RoleController : BaseApiController
    {
        private readonly IRoleManager _roleManager;

        public RoleController(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        #region CRUD operations

        [HttpGet]
        [Route("GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var response = await _roleManager.GetAllRolesAsync();
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IHttpActionResult> GetById(int? id)
        {
            // Validate if ID is missing or invalid
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("Please provide a valid Role ID.");
            }

            var response = await _roleManager.GetRoleByIdAsync(id.Value);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IHttpActionResult> Add([FromBody] RoleDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Role data cannot be null.");
            }

            if (string.IsNullOrEmpty(dto.Name))
            {
                return BadRequest("Role Name is required.");
            }

            var response = await _roleManager.AddRoleAsync(dto, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] RoleDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Role data cannot be null.");
            }

            if (dto.Id <= 0)
            {
                return BadRequest("A valid Role ID is required to update a record.");
            }

            var response = await _roleManager.UpdateRoleAsync(dto, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IHttpActionResult> Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("Please provide a valid Role ID to delete.");
            }

            var response = await _roleManager.DeleteRoleAsync(id.Value, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        #endregion CRUD operations
    }
}