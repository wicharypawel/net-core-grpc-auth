using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetCoreGrpc.JwtAuth.AspNetCoreServerApp.Services.Models;
using NetCoreGrpc.JwtAuth.Proto;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreGrpc.JwtAuth.AspNetCoreServerApp.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizationService : Authorization.AuthorizationBase
    {
        private readonly IConfiguration _configuration;

        public AuthorizationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        public override async Task<LoginReply> Login(LoginRequest loginViewModel, ServerCallContext context)
        {
            if (loginViewModel == null)
            {
                return new LoginReply() { IsSuccess = false };
            }
            if (loginViewModel.Login == "admin" && loginViewModel.Password == "password")
            {
                var user = new ApplicationUserDto
                {
                    Email = "wicharypawel@gmail.com",
                    FirstName = "Pawel",
                    LastName = "Wichary",
                    UserName = "admin",
                    Id = new Guid("248d037e-fc66-4544-bac7-363960143c8b").ToString()
                };
                var userRoles = new List<string>
                {
                    "User",
                    "Administrator"
                };
                var token = GetJwtToken(user, userRoles);
                return new LoginReply() { IsSuccess = true, JwtToken = token.JwtToken, Expiration = Timestamp.FromDateTime(token.Expiration) };
            }
            if (loginViewModel.Login == "user" && loginViewModel.Password == "password")
            {
                var user = new ApplicationUserDto
                {
                    Email = "jankowalski@test.com",
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    UserName = "user",
                    Id = new Guid("ec5d50f7-7b2d-4cad-bc8c-cf6d68093386").ToString()
                };
                var userRoles = new List<string>
                {
                    "User"
                };
                var token = GetJwtToken(user, userRoles);
                return new LoginReply() { IsSuccess = true, JwtToken = token.JwtToken, Expiration = Timestamp.FromDateTime(token.Expiration) };
            }
            return new LoginReply() { IsSuccess = false };
        }

        private TokenDto GetJwtToken(ApplicationUserDto applicationUserDto, IEnumerable<string> roles)
        {
            var applicationUserClaims = GetApplicationUserClaims(applicationUserDto);
            var applicationUserRolesClaims = GetRolesAsClaims(roles);
            var jwtAuthRequiredClaims = GetJwtAuthRequiredClaims(_configuration.GetValue<string>("JwtIssuer"), _configuration.GetValue<string>("JwtAudience"));
            var claims = jwtAuthRequiredClaims.Union(applicationUserRolesClaims).Union(applicationUserClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtKey")));
            var signingCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtHeader = new JwtHeader(signingCredential);
            var jwtPayload = new JwtPayload(claims);
            var token = new JwtSecurityToken(jwtHeader, jwtPayload);
            return new TokenDto
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        private static IEnumerable<Claim> GetApplicationUserClaims(ApplicationUserDto userDto)
        {
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDto.UserName),
                new Claim(JwtRegisteredClaimNames.GivenName, userDto.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, userDto.LastName),
                new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
                new Claim("MyOwnApplicationId", userDto.Id)
            };
        }

        private static IEnumerable<Claim> GetJwtAuthRequiredClaims(string issuer, string audience)
        {
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddHours(8)).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience)
            };
        }

        private static IEnumerable<Claim> GetRolesAsClaims(IEnumerable<string> roles)
        {
            const string roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            return roles.Select(x => new Claim(roleType, x));
        }
    }
}
