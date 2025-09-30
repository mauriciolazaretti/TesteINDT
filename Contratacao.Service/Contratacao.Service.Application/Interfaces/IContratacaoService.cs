using System;

namespace Contratacao.Service.Application.Interfaces;

public interface IContratacaoService
{
    Task<bool> ContratarSeguroAsync(Guid propostaId);
}
