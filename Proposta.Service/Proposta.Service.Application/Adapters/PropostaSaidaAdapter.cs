using System;
using Proposta.Service.Application.Interfaces;
using Proposta.Service.Application.Ports;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Domain.Repositories;
using Proposta.Service.Domain.Response;

namespace Proposta.Service.Application.Adapters;

public class PropostaSaidaAdapter : ISaidaPropostaPort
{
    private readonly IPropostaRepository _propostaRepository;

    public PropostaSaidaAdapter(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<bool> AtualizarStatusPropostaAsync(Guid id, StatusPropostaEnum status)
    {
        return await _propostaRepository.AtualizarStatusPropostaAsync(id, (int)status);
    }
}
