using System;
using Proposta.Service.Domain.Entities;

namespace Proposta.Service.Domain.Response;

public class ResponseSeguroProposta
{
    public List<SeguroProposta> Propostas { get; set; } = new List<SeguroProposta>();
    public int TotalPropostas { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
}