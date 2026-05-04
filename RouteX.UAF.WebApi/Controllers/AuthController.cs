using RouteX.UAF.Entities.DTOs;
using RouteX.UAF.LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RouteX.UAF.WebApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthManager _authManager;
        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] LoginDto dto)
        {

            dto.UserAgent = Request?.Headers?.UserAgent.ToString();
            dto.ipAddress = HttpContext.Current?.Request?.UserHostAddress ?? "Unknown IP";

            if (dto == null)
            {
                return BadRequest("Invalid login request.");
            }

            if (string.IsNullOrEmpty(dto.UafRegistrationNumber) && string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest("Either UAF Registration Number or Email must be provided.");
            }

            if (string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Password must be provided.");
            }

            if (string.IsNullOrEmpty(dto.DeviceToken))
            {
                return BadRequest("Device token is required for login.");
            }

            // Extracting BaseUrl from the current request
            dto.CurrentBaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            var response = _authManager.Login(dto);
            return Content((System.Net.HttpStatusCode)response.Code, response);
        }

        // Logout from all devices
        [HttpPost]
        [Route("logout/all")]
        public IHttpActionResult LogoutFromAllDevices()
        {
            // Extract token from header instead of body
            var accessToken = Request.Headers.Authorization?.Parameter;

            // The manager uses this token to find the UserId and wipe all sessions
            var response = _authManager.LogoutFromAllDevices(accessToken);
            return Content((HttpStatusCode)response.Code, response);
        }

        // Logout from the current device
        [HttpPost]
        [Route("logout")]
        public IHttpActionResult LogoutFromCurrentDevice()
        {
            var accessToken = Request.Headers.Authorization?.Parameter;

            // Get DeviceToken from a custom header (e.g., "X-Device-Token")
            IEnumerable<string> headerValues;
            string deviceToken = string.Empty;
            if (Request.Headers.TryGetValues("X-Device-Token", out headerValues))
            {
                deviceToken = headerValues.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(deviceToken))
            {
                return BadRequest("Device identifier is missing from headers.");
            }

            var response = _authManager.LogoutFromCurrentDevice(accessToken, deviceToken);
            return Content((HttpStatusCode)response.Code, response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody] RegisterStudentDto dto)
        {
            var response = await _authManager.RegisterStudentAsync(dto);
            return Content((System.Net.HttpStatusCode)response.Code, response);
        }

        [HttpPost]
        [Route("verify")]
        public IHttpActionResult VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var response = _authManager.VerifyOtp(dto);
            return Content((System.Net.HttpStatusCode)response.Code, response);
        }
    }
}