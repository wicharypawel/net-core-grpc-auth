using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreGrpc.CertAuth.AspNetCoreServerApp.App_Infrastructure.CertificateValidation;
using NetCoreGrpc.CertAuth.AspNetCoreServerApp.Services;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NetCoreGrpc.CertAuth.AspNetCoreServerApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICertificateValidationService, CertificateValidationService>(provider =>
            {
                var rootCertificatePublicKey = new X509Certificate2("ca.crt", "EsbSJh6YJTYkMvnPXDHLvHY8VFq7xN8z");
                return new CertificateValidationService(rootCertificatePublicKey);
            });
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(CertificateAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.RevocationMode = X509RevocationMode.NoCheck;
                options.Events = new CertificateAuthenticationEvents
                {
                    OnCertificateValidated = context =>
                    {
                        var validationService = context.HttpContext.RequestServices.GetService<ICertificateValidationService>();
                        if (validationService.ValidateCertificate(context.ClientCertificate))
                        {
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                            };
                            context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                            context.Success();
                        }
                        else
                        {
                            context.Fail("Certificate validation failed in ICertificateValidationService");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
