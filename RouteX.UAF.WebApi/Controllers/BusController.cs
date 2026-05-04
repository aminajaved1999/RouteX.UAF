using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.LogicLayer.Interfaces;
using RouteX.UAF.LogicLayer.Managers;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace RouteX.UAF.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Bus")]
    public class BusController : BaseApiController
    {
        private readonly IBusManager _busManager;

        public BusController(IBusManager busManager)
        {
            _busManager = busManager;
        }

        #region crud operations
        [HttpGet]
        [Route("GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var response = await _busManager.GetAllBusesAsync();
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IHttpActionResult> GetById(int? id)
        {
            // Validate if ID is missing or invalid
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("Please provide a valid Bus ID.");
            }

            var response = await _busManager.GetBusByIdAsync(id.Value);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IHttpActionResult> Add([FromBody] BusDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Bus data cannot be null.");
            }

            if (string.IsNullOrEmpty(dto.LicensePlate))
            {
                return BadRequest("LicensePlate is required");
            }

            var response = await _busManager.AddBusAsync(dto, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IHttpActionResult> Update([FromBody] BusDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Bus data cannot be null.");
            }

            if (dto.Id <= 0)
            {
                return BadRequest("A valid Bus ID is required to update a record.");
            }

            var response = await _busManager.UpdateBusAsync(dto, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IHttpActionResult> Delete(int? id) 
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("Please provide a valid Bus ID to delete.");
            }

            var response = await _busManager.DeleteBusAsync(id.Value, LoggedInUserId);
            return Content((HttpStatusCode)response.Code, response);
        }

        #endregion crud operations

    }
}