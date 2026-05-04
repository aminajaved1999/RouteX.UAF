using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace RouteX.UAF.WebApi.Controllers
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        #region User Context Properties

        /// <summary>
        /// The unique Database ID of the logged-in user.
        /// </summary>
        protected int LoggedInUserId => GetClaimValue<int>("UserId", int.TryParse);

        /// <summary>
        /// The Full Name of the user as stored in the JWT.
        /// </summary>
        protected string LoggedInUserName => GetClaimValue(ClaimTypes.Name, StringParser);

        /// <summary>
        /// The Role name (e.g., "Admin", "Student") assigned to the user.
        /// </summary>
        protected string LoggedInUserRole => GetClaimValue(ClaimTypes.Role, StringParser);

        /// <summary>
        /// The unique Session ID associated with this specific login.
        /// </summary>
        protected string CurrentSessionId => GetClaimValue("SessionId", StringParser);

        /// <summary>
        /// The User's email address.
        /// </summary>
        protected string LoggedInUserEmail => GetClaimValue(ClaimTypes.Email, StringParser);

        #endregion

        #region Claim Parsing Logic

        /// <summary>
        /// Internal helper to extract and parse claim values.
        /// </summary>
        private T GetClaimValue<T>(string claimType, TryParseHandler<T> parser)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw CreateUnauthorizedException("Identity context is null. User may not be authenticated.");
            }

            // Look for the claim by custom name, standard URI, or JWT registered name
            var claim = identity.Claims.FirstOrDefault(c =>
                c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) ||
                c.Type.Equals(GetStandardClaimMapping(claimType), StringComparison.OrdinalIgnoreCase));

            if (claim != null && parser(claim.Value, out T result))
            {
                return result;
            }

            throw CreateUnauthorizedException($"Required security claim '{claimType}' is missing or malformed.");
        }

        /// <summary>
        /// Maps common keys to standard ClaimTypes for broader compatibility.
        /// </summary>
        private string GetStandardClaimMapping(string key)
        {
            switch (key.ToLower())
            {
                case "userid": return JwtRegisteredClaimNames.Sub;
                case "role": return ClaimTypes.Role;
                case "name": return ClaimTypes.Name;
                case "email": return ClaimTypes.Email;
                default: return key;
            }
        }

        #endregion

        #region Response Helpers

        /// <summary>
        /// Creates a structured 401 Unauthorized exception with a JSON body.
        /// </summary>
        private HttpResponseException CreateUnauthorizedException(string message)
        {
            var errorResponse = Request.CreateResponse(HttpStatusCode.Unauthorized, new
            {
                Success = false,
                Code = 401,
                Message = message,
                Timestamp = DateTime.UtcNow
            });

            return new HttpResponseException(errorResponse);
        }

        // Delegate for parsing logic (supports int.TryParse, etc.)
        private delegate bool TryParseHandler<T>(string value, out T result);

        // Standard parser for strings
        private static readonly TryParseHandler<string> StringParser = (string val, out string res) =>
        {
            res = val;
            return !string.IsNullOrWhiteSpace(val);
        };

        #endregion
    }
}