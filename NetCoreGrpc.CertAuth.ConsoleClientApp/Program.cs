using System;
using System.IO;
using Grpc.Core;
using NetCoreGrpc.CertAuth.Proto;

namespace NetCoreGrpc.CertAuth.ConsoleClientApp
{
    // based on: https://stackoverflow.com/questions/37714558/how-to-enable-server-side-ssl-for-grpc/37739265#37739265
    public class Program
    {
        public static void Main()
        {
            var cacert = File.ReadAllText("ca.crt");
            var clientcert = File.ReadAllText("client.crt");
            var clientkey = File.ReadAllText("client.key");
            var ssl = new SslCredentials(cacert, new KeyCertificatePair(clientcert, clientkey));
            var channel = new Channel("localhost", 5001, ssl);
            var client = new Greeter.GreeterClient(channel);
            var user = "Pawel";
            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
