using Proposta.Service.Application.Interfaces;
using Proposta.Service.API.DTO;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Proposta.Service.Domain.Services;
using Proposta.Service.Application.Ports;
using Proposta.Service.Application.Adapters;
using Proposta.Service.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();


builder.Services.AddScoped<IPropostaService, PropostaService>();
builder.Services.AddScoped<IEntradaPropostaPort, PropostaEntradaAdapter>();
builder.Services.AddScoped<ISaidaPropostaPort, PropostaSaidaAdapter>();
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapPost("/propostas", async (CriarPropostaDTO dto, IPropostaService propostaService) =>
{
    var proposta = new SeguroProposta
    {
        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
        Data = dto.Data,
        Segurado = dto.Segurado,
        Produto = dto.Produto,
        Valor = dto.Valor,
        Status = (int)dto.Status
    };

    await propostaService.CriarPropostaAsync(proposta);
    return Results.Created($"/propostas/{proposta.Id}", proposta);
})
.WithName("CriarProposta")
.WithSummary("Criar uma nova proposta")
.Produces(201)
.Produces(400);

app.MapGet("/propostas/{id:guid}", async (Guid id, IPropostaService propostaService) =>
{
    var proposta = await propostaService.ObterPropostaPorIdAsync(id);
    return proposta is not null ? Results.Ok(proposta) : Results.NotFound();
})
.WithName("ObterPropostaPorId")
.WithSummary("Obter proposta por ID")
.Produces<SeguroProposta>(200)
.Produces(404);

app.MapGet("/propostas", async ([FromQuery] int pagina, [FromQuery] int tamanhoPagina, IPropostaService propostaService) =>
{
    var resultado = await propostaService.ObterTodasPropostasAsync(pagina, tamanhoPagina);
    return Results.Ok(resultado);
})
.WithName("ObterTodasPropostas")
.WithSummary("Obter todas as propostas com paginação")
.Produces<Proposta.Service.Domain.Response.ResponseSeguroProposta>(200);

app.MapPut("/propostas/{id:guid}/status", async (Guid id, AlterarStatusPropostaDTO dto, IPropostaService propostaService) =>
{
    var sucesso = await propostaService.AtualizarStatusPropostaAsync(id, (StatusPropostaEnum)dto.Status);
    return sucesso ? Results.Ok() : Results.NotFound();
})
.WithName("AtualizarStatusProposta")
.WithSummary("Atualizar status de uma proposta")
.Produces(200)
.Produces(404);

app.Run();
