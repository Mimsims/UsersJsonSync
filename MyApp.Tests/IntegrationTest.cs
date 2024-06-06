using Funq;
using ServiceStack;
using NUnit.Framework;
using MyApp.ServiceInterface;
using MyApp.ServiceModel;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Tests;

public class IntegrationTest
{
    const string BaseUri = "http://localhost:2000/";
    private readonly ServiceStackHost appHost;

    class AppHost : AppSelfHostBase
    {
        public AppHost() : base(nameof(IntegrationTest), typeof(RemoteToLocalServices).Assembly) { }

        public override void Configure(Container container)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectModels;Database=UsersEvoDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            container.Register<UserDbContext>(c => new UserDbContext(optionsBuilder.Options)).ReusedWithin(ReuseScope.Request);
        }
    }

    public IntegrationTest()
    {
        appHost = new AppHost()
            .Init()
            .Start(BaseUri);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

    [Test]
    public void Can_call_RemoteToLocal_Service()
    {
        var client = CreateClient();

        var response = client.Get(new RemoteToLocalRequest { });

        Assert.That(response.Result, Is.EqualTo("Success: Remote API synced to Local DB!"));
    }

    [Test]
    public void Can_call_LocalToRemoteServices()
    {
        var client = CreateClient();

        var response = client.Get(new LocalToRemoteRequest { });

        Assert.That(response.Result, Is.EqualTo("Success: Local DB synced to Remote API!"));
    }

    [Test]
    public void Can_call_UpdateFieldServices()
    {
        var client = CreateClient();

        var response = client.Get(new UpdateFieldRequest { Email = "integration@test.test" });

        Assert.That(response.Result, Is.EqualTo("Success: Email field updated to 10 users integration@test.test"));
    }

    [Test]
    public void Can_call_DeleteServices_with_nonExistID()
    {
        var client = CreateClient();

        var response = client.Get(new DeleteRequest { Id = 10000 });

        Assert.That(response.Result, Is.EqualTo("Error: user 10000 not found"));
    }

    [Test]
    public void Can_call_DeleteServices()
    {
        var client = CreateClient();

        var response = client.Get(new DeleteRequest { Id = 2 });

        Assert.That(response.Result, Is.EqualTo("Success: user 2 deleted"));
    }
    
}