using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RouteX.UAF.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ========================================================
            // 1. Configure CORS (Cross-Origin Resource Sharing)
            // ========================================================
            // Parameters: origins, headers, methods
            // For MVP/Development: We use "*" to allow everything.
            // For Production: Change "*" to "https://your-admin-portal.com"
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // ========================================================
            // 2. Web API configuration and services
            // ========================================================
            config.MessageHandlers.Add(new JwtAuthenticationHandler());

            // ========================================================
            // 3. Web API routes
            // ========================================================
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // ========================================================
            // 4. JSON Serialization Settings
            // ========================================================
            // Ignore Null values globally to keep payloads small
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // Ignore Default values globally 
            config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
        }
    }
}