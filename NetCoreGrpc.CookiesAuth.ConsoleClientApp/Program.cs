using System;
using Grpc.Net.Client;
using NetCoreGrpc.CookiesAuth.Proto;

namespace NetCoreGrpc.CookiesAuth.ConsoleClientApp
{
    // based on: https://stackoverflow.com/questions/37714558/how-to-enable-server-side-ssl-for-grpc/37739265#37739265
    public class Program
    {
        public static void Main()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var authClient = new Authorization.AuthorizationClient(channel);
            authClient.Login(new LoginRequest() { Login = "user", Password = "password" }); // cookie is already set after this call
            var user = "Pawel";
            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
