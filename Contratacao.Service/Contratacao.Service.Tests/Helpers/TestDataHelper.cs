using Contratacao.Service.Domain.Entities;
using Contratacao.Service.Infrastructure.Entities;

namespace Contratacao.Service.Tests.Helpers;

public static class TestDataHelper
{
    public static List<ContratacaoInfraSeguro> GetContratacaoPropostasTestData()
    {
        return new List<ContratacaoInfraSeguro>
        {
            new ContratacaoInfraSeguro
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                PropostaId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Data = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            },
            new ContratacaoInfraSeguro
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                PropostaId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Data = new DateTime(2024, 2, 1, 11, 0, 0, DateTimeKind.Utc)
            },
            new ContratacaoInfraSeguro
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                PropostaId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Data = new DateTime(2024, 3, 1, 12, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    public static List<ContratacaoProposta> GetContratacaoPropostasDomainTestData()
    {
        return new List<ContratacaoProposta>
        {
            new ContratacaoProposta
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                PropostaId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Data = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            },
            new ContratacaoProposta
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                PropostaId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Data = new DateTime(2024, 2, 1, 11, 0, 0, DateTimeKind.Utc)
            },
            new ContratacaoProposta
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                PropostaId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Data = new DateTime(2024, 3, 1, 12, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    public static ContratacaoProposta GetValidContratacaoProposta()
    {
        return new ContratacaoProposta
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            PropostaId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            Data = DateTime.UtcNow
        };
    }

    public static SeguroProposta GetValidSeguroProposta(int status = 1)
    {
        return new SeguroProposta
        {
            Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            Status = status
        };
    }
}