using System;
using System.Security.Cryptography.X509Certificates;

namespace NetCoreGrpc.CertAuth.AspNetCoreServerApp.App_Infrastructure.CertificateValidation
{
    public sealed class CertificateValidationService : ICertificateValidationService
    {
        private readonly X509Certificate2 _authority;

        public CertificateValidationService(X509Certificate2 authority)
        {
            _authority = authority ?? throw new ArgumentNullException(nameof(authority));
            if (_authority.HasPrivateKey)
            {
                throw new ArgumentException("Authority private key isn't required during verification.");
            }
        }

        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.ExtraStore.Add(_authority);
            if (!chain.Build(clientCertificate)) return false;
            var isAuthorityRoot = chain.ChainElements[chain.ChainElements.Count - 1].Certificate.Thumbprint.Equals(_authority.Thumbprint);
            return isAuthorityRoot;
        }
    }
}
