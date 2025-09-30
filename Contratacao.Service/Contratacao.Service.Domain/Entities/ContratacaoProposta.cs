using System;

namespace Contratacao.Service.Domain.Entities;

public class ContratacaoProposta
{
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
}
