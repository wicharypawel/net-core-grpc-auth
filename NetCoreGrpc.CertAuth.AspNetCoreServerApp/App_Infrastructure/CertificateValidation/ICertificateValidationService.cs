using System.Security.Cryptography.X509Certificates;

namespace NetCoreGrpc.CertAuth.AspNetCoreServerApp.App_Infrastructure.CertificateValidation
{
    public interface ICertificateValidationService
    {
        bool ValidateCertificate(X509Certificate2 clientCertificate);
    }
}
