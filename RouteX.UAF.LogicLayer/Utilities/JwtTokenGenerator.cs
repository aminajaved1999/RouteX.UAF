using Microsoft.IdentityModel.Tokens;
using RouteX.UAF.Entities.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RouteX.UAF.LogicLayer.Utilities
{
    public static class JwtTokenGenerator
    {
        // In production, keep these in Web.config <appSettings>
        private const string SecretKey = "RouteX_UAF_Super_Secret_Security_Key_2026_Do_Not_Share";
        private const string Issuer = "RouteX.UAF.Backend";
        private const string Audience = "RouteX.UAF.MobileApp";

        public static string GenerateToken(int userId, string fullName, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims are pieces of info about the user encoded into the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}