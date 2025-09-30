using System;
using Contratacao.Service.Domain.Entities;
using Contratacao.Service.Domain.Repositories;
using Contratacao.Service.Infrastructure.Context;
using Contratacao.Service.Infrastructure.Entities;

namespace Contratacao.Service.Infrastructure.Repositories;

public class ContratacaoRepository : IContratacaoRepository
{
    private readonly AppDbContext _appDbContext;
    public ContratacaoRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<bool> SalvarSeguroPropostaAsync(ContratacaoProposta contratacaoProposta)
    {
        var infraEntity = new ContratacaoInfraSeguro
        {
            Id = contratacaoProposta.Id,
            PropostaId = contratacaoProposta.PropostaId,
            Data = contratacaoProposta.Data
        };
        await _appDbContext.ContratacaoPropostas.AddAsync(infraEntity);
        var result = await _appDbContext.SaveChangesAsync();

        return result > 0;
    }
}
