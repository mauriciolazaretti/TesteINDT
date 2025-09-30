using System;

namespace Proposta.Service.Infrastructure.Entities;

public class PropostaInfraEntity
{
    public Guid Id { get; set; }
    public string Segurado { get; set; } = "";
    public decimal Valor { get; set; }
    public int Status { get; set; }
    public string Produto { get; set; } = "";
    public DateTime Data { get; set; } = DateTime.UtcNow;
}