using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Proposta.Service.Infrastructure.Context;

namespace Proposta.Service.Tests.Helpers;

public static class DbContextHelper
{
    public static AppDbContext CreateInMemoryContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var configurationData = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "InMemoryDatabase"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        return new TestAppDbContext(options, configuration);
    }
}

public class TestAppDbContext : AppDbContext
{
    private readonly DbContextOptions<AppDbContext> _options;

    public TestAppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) 
        : base(configuration)
    {
        _options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
}