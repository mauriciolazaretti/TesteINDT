using System;

namespace Proposta.Service.API.DTO;

public class ObterListaPropostaDTO
{
    public List<ObterPropostaDTO> Propostas { get; set; } = new();
    public int TotalDePropostas { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalDePaginas { get; set; }
    public int ItensPorPagina { get; set; }
}
