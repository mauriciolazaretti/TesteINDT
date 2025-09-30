using System;

namespace Contratacao.Service.Application.Ports;

public interface IPropostaServicePort
{
    Task<bool> VerificarPropostaAprovadaAsync(Guid id);
}
