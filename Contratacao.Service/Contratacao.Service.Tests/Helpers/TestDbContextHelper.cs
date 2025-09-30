using Microsoft.EntityFrameworkCore;
using Contratacao.Service.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Contratacao.Service.Tests.Helpers;

public static class TestDbContextHelper
{
    public static AppDbContext CreateInMemoryDbContext()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static AppDbContext CreateInMemoryDbContextWithData()
    {
        var context = CreateInMemoryDbContext();
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(AppDbContext context)
    {
        var testData = TestDataHelper.GetContratacaoPropostasTestData();
        context.ContratacaoPropostas.AddRange(testData);
        context.SaveChanges();
    }
}