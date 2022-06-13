# RavenDB repository for `AspNetCore.DataProtection`

Package to enable persisting keys from `AspNetCore.DataProtection` to `RavenDB`.

## Usage

The package is available from Nuget as `DataProtection.AspNetCore.RavenDB`.  E.g.

```c#
using AspNetCore.DataProtection.RavenDB;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDataProtection()
    .PersistKeysToRavenDB(new [] {"https://ravendb.example.com" },
        "DataProtectionKeys", 
        new X509Certificate2("client.pfx"));
```

## License

This repository is licensed under the [MIT License.](LICENSE)
