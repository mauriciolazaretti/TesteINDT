using System;
using Microsoft.EntityFrameworkCore;
using Proposta.Service.Domain.Repositories;
using Proposta.Service.Domain.Response;
using Proposta.Service.Infrastructure.Context;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Infrastructure.Entities;

namespace Proposta.Service.Infrastructure.Repositories;

public class PropostaRepository : IPropostaRepository
{
    private readonly AppDbContext _context;
    public PropostaRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<SeguroProposta> CriarPropostaAsync(SeguroProposta seguroProposta)
    {
        var infraEntity = new PropostaInfraEntity
        {
            Id = seguroProposta.Id,
            Segurado = seguroProposta.Segurado,
            Valor = seguroProposta.Valor,
            Produto = seguroProposta.Produto,
            Data = seguroProposta.Data,
            Status = seguroProposta.Status
        };
        
        await _context.AddAsync(infraEntity);
        await _context.SaveChangesAsync();
        
        return new SeguroProposta
        {
            Id = infraEntity.Id,
            Segurado = infraEntity.Segurado,
            Valor = infraEntity.Valor,
            Produto = infraEntity.Produto,
            Data = infraEntity.Data,
            Status = infraEntity.Status
        };
    }

    public async Task<SeguroProposta?> ObterPropostaPorIdAsync(Guid id)
    {
        var infraEntity = await _context.Propostas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (infraEntity is null) return null;
        
        return new SeguroProposta
        {
            Id = infraEntity.Id,
            Segurado = infraEntity.Segurado,
            Valor = infraEntity.Valor,
            Produto = infraEntity.Produto,
            Data = infraEntity.Data,
            Status = infraEntity.Status
        };
    }

    public async Task<ResponseSeguroProposta> ObterTodasPropostasAsync(int pagina, int tamanhoPagina)
    {
        var query = _context.Propostas.AsNoTracking();

        var totalRegistros = await query.CountAsync();
        var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanhoPagina);

        var propostas = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new ResponseSeguroProposta
        {
            PaginaAtual = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            TotalPropostas = totalRegistros,
            Propostas = propostas.Select(p => new SeguroProposta
            {
                Id = p.Id,
                Segurado = p.Segurado,
                Valor = p.Valor,
                Produto = p.Produto,
                Data = p.Data,
                Status = p.Status
            }).ToList()
        };
    }

    public async Task<bool> AtualizarStatusPropostaAsync(Guid id, int status)
    {
        var proposta = await _context.Propostas.FirstOrDefaultAsync(p => p.Id == id);
        if (proposta is null) return false;
        
        proposta.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
