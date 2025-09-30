using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proposta.Service.Domain.Repositories;
using Proposta.Service.Infrastructure.Context;
using Proposta.Service.Infrastructure.Repositories;

namespace Proposta.Service.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.BuildServiceProvider().GetService<AppDbContext>()?.Database.Migrate();
        services.AddScoped<IPropostaRepository, PropostaRepository>();
        return services;
    }
}