using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Infrastructure.Entities;

namespace Proposta.Service.Tests.Helpers;

public static class TestDataHelper
{


    public static SeguroProposta CriarSeguroPropostaValida()
    {
        return new SeguroProposta
        {
            Id = Guid.NewGuid(),
            Data = DateTime.Now,
            Segurado = "Carlos Oliveira",
            Produto = "Seguro Vida",
            Valor = 2000.00m,
            Status = (int)StatusPropostaEnum.EmAnalise
        };
    }

    public static SeguroProposta CriarSeguroPropostaComId(Guid id)
    {
        return new SeguroProposta
        {
            Id = id,
            Data = DateTime.Now,
            Segurado = "Ana Costa",
            Produto = "Seguro Auto",
            Valor = 1200.00m,
            Status = (int)StatusPropostaEnum.Rejeitada
        };
    }

    public static PropostaInfraEntity CriarPropostaInfraEntityValida()
    {
        return new PropostaInfraEntity
        {
            Id = Guid.NewGuid(),
            Data = DateTime.Now,
            Segurado = "Pedro Ferreira",
            Produto = "Seguro Residencial",
            Valor = 1800.00m,
            Status = (int)StatusPropostaEnum.EmAnalise
        };
    }

    public static PropostaInfraEntity CriarPropostaInfraEntityComId(Guid id)
    {
        return new PropostaInfraEntity
        {
            Id = id,
            Data = DateTime.Now,
            Segurado = "Lucia Almeida",
            Produto = "Seguro Saúde",
            Valor = 950.00m,
            Status = (int)StatusPropostaEnum.Aprovada
        };
    }

    public static List<PropostaInfraEntity> CriarListaPropostasInfra(int quantidade)
    {
        var propostas = new List<PropostaInfraEntity>();
        for (int i = 0; i < quantidade; i++)
        {
            propostas.Add(new PropostaInfraEntity
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now,
                Segurado = $"Segurado {i + 1}",
                Produto = $"Produto {i + 1}",
                Valor = 1000.00m + i * 100,
                Status = i % 3 // Varia entre 0, 1, 2 (EmAnalise, Aprovada, Rejeitada)
            });
        }
        return propostas;
    }
}