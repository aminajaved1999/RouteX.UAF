using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Tokens;

public class JwtAuthenticationHandler : DelegatingHandler
{
    private const string SecretKey = "RouteX_UAF_Super_Secret_Security_Key_2026_Do_Not_Share";
    private const string Issuer = "RouteX.UAF.Backend";
    private const string Audience = "RouteX.UAF.MobileApp";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = request.Headers.Authorization;

        if (authHeader != null && authHeader.Scheme == "Bearer")
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(authHeader.Parameter, validationParameters, out validatedToken);

                // Set the principal for the current request
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
            catch (Exception)
            {
                // Token validation failed
                return request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}