using System;

namespace Proposta.Service.Domain.Entities;

public class SeguroProposta
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public string Segurado { get; set; } = "";
    public string Produto { get; set; } = "";
    public decimal Valor { get; set; }
    public int Status { get; set; }
}