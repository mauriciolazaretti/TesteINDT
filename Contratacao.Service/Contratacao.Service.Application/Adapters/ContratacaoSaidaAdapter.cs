using System;
using Contratacao.Service.Application.Ports;
using Contratacao.Service.Domain.Entities;
using Contratacao.Service.Domain.Repositories;

namespace Contratacao.Service.Application.Adapters;

public class ContratacaoSaidaAdapter : IContratacaoSaidaPort
{
    private readonly IContratacaoRepository _contratacaoRepository;

    public ContratacaoSaidaAdapter(IContratacaoRepository contratacaoRepository)
    {
        _contratacaoRepository = contratacaoRepository;
    }

    public async Task<bool> SalvarSeguroPropostaAsync(ContratacaoProposta contratacaoProposta)
    {
        return await _contratacaoRepository.SalvarSeguroPropostaAsync(contratacaoProposta);
    }
}
