using System;
using Proposta.Service.API.Enums;

namespace Proposta.Service.API.DTO;

public class CriarPropostaDTO
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public string Segurado { get; set; } = "";
    public string Produto { get; set; } = "";
    public decimal Valor { get; set; }
    public StatusPropostaEnum Status { get; set; } = StatusPropostaEnum.EmAnalise;
}
