using System;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using NetCoreGrpc.CertAuth.Proto;

namespace NetCoreGrpc.CertAuth.ConsoleServerApp
{
    // based on: https://stackoverflow.com/questions/37714558/how-to-enable-server-side-ssl-for-grpc/37739265#37739265
    public class Program
    {
        const int Port = 5001;

        public static void Main()
        {
            var cacert = File.ReadAllText("ca.crt");
            var servercert = File.ReadAllText("server.crt");
            var serverkey = File.ReadAllText("server.key");
            var keypair = new KeyCertificatePair(servercert, serverkey);
            var clientCertRequest = SslClientCertificateRequestType.RequestAndRequireAndVerify;
            var sslCredentials = new SslServerCredentials(new List<KeyCertificatePair>() { keypair }, cacert, clientCertRequest);
            var server = new Server
            {
                Services = { Greeter.BindService(new GreeterService()) },
                Ports = { new ServerPort("localhost", Port, sslCredentials) }
            };
            server.Start();
            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
    }
}
