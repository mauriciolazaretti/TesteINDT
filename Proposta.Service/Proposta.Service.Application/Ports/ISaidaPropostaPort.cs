using System;
using Proposta.Service.Domain.Enums;

namespace Proposta.Service.Application.Ports;

public interface ISaidaPropostaPort
{

    Task<bool> AtualizarStatusPropostaAsync(Guid id, StatusPropostaEnum status);
}
