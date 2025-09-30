using System;
using Proposta.Service.API.Enums;

namespace Proposta.Service.API.DTO;

public class AlterarStatusPropostaDTO
{
    Guid Id { get; set; }
    public StatusPropostaEnum Status { get; set; }
}
