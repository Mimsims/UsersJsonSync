using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using MyApp.ServiceInterface;
using MyApp.ServiceModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Tests;

public class UnitTest
{
    private readonly ServiceStackHost appHost;

    public UnitTest()
    {

        appHost = new BasicAppHost().Init();

        appHost.Container.AddTransient<UserDbContext>(() =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectModels;Database=UsersEvoDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new UserDbContext(optionsBuilder.Options);
        });        
        appHost.Container.AddTransient<RemoteToLocalServices>();
        appHost.Container.AddTransient<LocalToRemoteServices>();
        appHost.Container.AddTransient<UpdateFieldServices>();
        appHost.Container.AddTransient<DeleteServices>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    [Test]
    public async Task Can_call_RemoteToLocalServices()
    {
        var service = appHost.Container.Resolve<RemoteToLocalServices>();

        var response = (MessageResponse)await service.AnyAsync(new RemoteToLocalRequest { });

        Assert.That(response.Result, Is.EqualTo("Success: Remote API synced to Local DB!"));
    }

    [Test]
    public async Task Can_call_LocalToRemoteServices()
    {
        var service = appHost.Container.Resolve<LocalToRemoteServices>();

        var response = (MessageResponse)await service.AnyAsync(new LocalToRemoteRequest { });

        Assert.That(response.Result, Is.EqualTo("Success: Local DB synced to Remote API!"));
    }

    [Test]
    public async Task Can_call_UpdateFieldServices()
    {
        var service = appHost.Container.Resolve<UpdateFieldServices>();

        var response = (MessageResponse)await service.AnyAsync(new UpdateFieldRequest { Email = "test@test.test" });

        Assert.That(response.Result, Is.EqualTo("Success: Email field updated to 10 users test@test.test"));
    }

    [Test]
    public async Task Can_call_DeleteServices_with_nonExistID()
    {
        var service = appHost.Container.Resolve<DeleteServices>();

        var response = (MessageResponse)await service.AnyAsync(new DeleteRequest { Id = 999 });

        Assert.That(response.Result, Is.EqualTo("Error: user 999 not found"));
    }

    [Test]
    public async Task Can_call_DeleteServices()
    {
        var service = appHost.Container.Resolve<DeleteServices>();

        var response = (MessageResponse)await service.AnyAsync(new DeleteRequest { Id = 1 });

        Assert.That(response.Result, Is.EqualTo("Success: user 1 deleted"));
    }
}