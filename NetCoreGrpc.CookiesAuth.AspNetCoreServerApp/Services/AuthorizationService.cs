using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using NetCoreGrpc.CookiesAuth.Proto;
using NetCoreGrpc.CookiesAuth.AspNetCoreServerApp.Services.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using Google.Protobuf.WellKnownTypes;

namespace NetCoreGrpc.CookiesAuth.AspNetCoreServerApp.Services
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
                var claimsPrincipal = GetUserClaimsPrincipal(user, userRoles);
                var authProperties = new AuthenticationProperties()
                {
                    IssuedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    IsPersistent = false
                };
                var httpContext = context.GetHttpContext();
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                return new LoginReply() { IsSuccess = true , Expiration = Timestamp.FromDateTimeOffset(authProperties.ExpiresUtc.Value) };
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
                var claimsPrincipal = GetUserClaimsPrincipal(user, userRoles);
                var authProperties = new AuthenticationProperties()
                {
                    IssuedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    IsPersistent = false
                };
                var httpContext = context.GetHttpContext();
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                return new LoginReply() { IsSuccess = true, Expiration = Timestamp.FromDateTimeOffset(authProperties.ExpiresUtc.Value) };
            }
            return new LoginReply() { IsSuccess = false };
        }

        [Authorize]
        public override async Task<Empty> Logout(Empty request, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new Empty();
        }

        private ClaimsPrincipal GetUserClaimsPrincipal(ApplicationUserDto applicationUserDto, IEnumerable<string> roles)
        {
            var applicationUserClaims = GetApplicationUserClaims(applicationUserDto);
            var applicationUserRolesClaims = GetRolesAsClaims(roles);
            var claims = applicationUserRolesClaims.Union(applicationUserClaims);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        private static IEnumerable<Claim> GetApplicationUserClaims(ApplicationUserDto userDto)
        {
            return new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.UserName),
                new Claim(ClaimTypes.Name, userDto.FirstName),
                new Claim(ClaimTypes.Surname, userDto.LastName),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim("MyOwnApplicationId", userDto.Id)
            };
        }

        private static IEnumerable<Claim> GetRolesAsClaims(IEnumerable<string> roles)
        {
            const string roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            return roles.Select(x => new Claim(roleType, x));
        }
    }
}
