using Contratacao.Service.API.DTO;
using Contratacao.Service.Application.Adapters;
using Contratacao.Service.Application.Interfaces;
using Contratacao.Service.Application.Ports;
using Contratacao.Service.Application.Services;
using Contratacao.Service.Infrastructure.Context;
using Contratacao.Service.Infrastructure.DI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IContratacaoService, ContratacaoService>();
builder.Services.AddScoped<IPropostaServicePort, PropostaServiceAdapter>();
builder.Services.AddScoped<IContratacaoSaidaPort, ContratacaoSaidaAdapter>();
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Testando conexão...");

    // Testar conexão primeiro
    var canConnect = await context.Database.CanConnectAsync();
    if (!canConnect)
    {
        logger.LogError("Não conseguiu conectar ao banco de dados");
        throw new InvalidOperationException("Cannot connect to database");
    }

    logger.LogInformation("Conexão bem sucedida");

    // Criar banco se não existir e aplicar migrations
    logger.LogInformation("Garantindo que o banco de dados seja criado...");
    await context.Database.MigrateAsync();

}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Já inicializou: {Message}", ex.Message);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/contratar", async (ContratarSeguroDTO dto, IContratacaoService contratacaoService) =>
{
    var result = await contratacaoService.ContratarSeguroAsync(dto.PropostaId);
    return result ? Results.Ok() : Results.BadRequest("Não foi possível contratar o seguro para a proposta informada.");
});

app.Run();