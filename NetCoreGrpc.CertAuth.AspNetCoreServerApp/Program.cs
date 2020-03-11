using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace NetCoreGrpc.CertAuth.AspNetCoreServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 5001, listenOptions =>
                        {
                            listenOptions.UseHttps(options =>
                            {
                                options.ClientCertificateMode = ClientCertificateMode.AllowCertificate; // Client-authenticated TLS handshake
                                options.ClientCertificateValidation = (_, __, ___) => true; // client certificate will be validated by middleware
                                options.ServerCertificate = new X509Certificate2("server.pfx", "EsbSJh6YJTYkMvnPXDHLvHY8VFq7xN8z"); // custom server cert
                            });
                        });
                    });
                });
    }
}
