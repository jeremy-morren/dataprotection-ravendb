using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace AspNetCore.DataProtection.RavenDB.Tests;

public class RavenDBXmlRepositoryTests : RavenTestDriver
{
    [Fact]
    public void RoundTrip()
    {
        using var store = GetDocumentStore();
        using var services = new ServiceCollection()
            .AddDataProtection()
            .PersistKeysToRavenDB(() => GetDocumentStore())
            .Services.BuildServiceProvider();
        var protector = services.GetRequiredService<IDataProtectionProvider>()
            .CreateProtector(nameof(RoundTrip));
        
        var payload = Guid.NewGuid().ToString();
        Assert.Equal(payload, protector.Unprotect(protector.Protect(payload)));
    }

    protected override void PreInitialize(IDocumentStore documentStore)
    {
        documentStore.SetConventions();
        base.PreInitialize(documentStore);
    }
}