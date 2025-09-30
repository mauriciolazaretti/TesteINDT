using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Contratacao.Service.Tests.Helpers;

public static class MockHelper
{
    public static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    public static Mock<IConfiguration> CreateMockConfiguration(Dictionary<string, string> configurationValues)
    {
        var mockConfiguration = new Mock<IConfiguration>();
        
        foreach (var kvp in configurationValues)
        {
            mockConfiguration.Setup(x => x[kvp.Key]).Returns(kvp.Value);
        }
        
        return mockConfiguration;
    }

    public static IConfiguration CreateConfiguration(Dictionary<string, string> configurationValues)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(configurationValues);
        return configurationBuilder.Build();
    }
}