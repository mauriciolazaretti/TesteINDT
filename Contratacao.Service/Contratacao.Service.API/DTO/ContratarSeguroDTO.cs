using System;

namespace Contratacao.Service.API.DTO;

public class ContratarSeguroDTO
{
    public Guid PropostaId { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
}
