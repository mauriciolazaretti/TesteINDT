using System;

namespace Contratacao.Service.Infrastructure.Entities;

public class ContratacaoInfraSeguro
{
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
}
