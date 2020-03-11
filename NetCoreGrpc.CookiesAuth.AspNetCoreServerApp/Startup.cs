using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreGrpc.CookiesAuth.AspNetCoreServerApp.Services;
using System;
using System.Threading.Tasks;

namespace NetCoreGrpc.CookiesAuth.AspNetCoreServerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                var unauthorizedRedirect = new Func<RedirectContext<CookieAuthenticationOptions>, Task>((context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                });
                var forbiddenRedirect = new Func<RedirectContext<CookieAuthenticationOptions>, Task>((context) =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                });
                options.Events.OnRedirectToLogin = unauthorizedRedirect;
                options.Events.OnRedirectToLogout = unauthorizedRedirect;
                options.Events.OnRedirectToReturnUrl = unauthorizedRedirect;
                options.Events.OnRedirectToAccessDenied = forbiddenRedirect;
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
                endpoints.MapGrpcService<AuthorizationService>();
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
