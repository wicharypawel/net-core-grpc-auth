# Net Core gRPC Auth

# This repository

This repository shows how to protect gRPC services from unauthenticated use. 

Two official types of authentication (Credential types) are:
- channel credentials (client auth data is once when channel is created)
- call credentials (client auth data is passed every call)

More: https://grpc.io/docs/guides/auth/

Repository authentication examples are:
- certificate authentication (implementation of channel credentials)
- jwt authentication (implementation of call credentials)
- cookies authentication (implementation of channel credentials) (UNOFFICIAL IMPLEMENTATION)

IMPORTANT: Certificate files used in this repository are just an example do not use it in production.
IMPORTANT: Assume that Client certifiate is derived from CA certificate
IMPORTANT: Assume that Server certifiate is derived from CA certificate

## Getting started

1. Download repository 
2. Download .Net SDK (in the moment of writing 3.1.101)
3. Open in VS 2019 or newer
4. Setup startup project's using multiple startup & start debugging

IMPORTANT: `NetCoreGrpc.CertAuth.AspNetCoreServerApp` has additional `README.md` file

## Cleanup

- not apply

## Sources

- https://grpc.io/docs/guides/auth/