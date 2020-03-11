using System;

namespace NetCoreGrpc.JwtAuth.AspNetCoreServerApp.Services.Models
{
    public sealed class TokenDto
    {
        public string JwtToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
