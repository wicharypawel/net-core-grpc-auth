namespace NetCoreGrpc.JwtAuth.AspNetCoreServerApp.Services.Models
{
    public sealed class ApplicationUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
