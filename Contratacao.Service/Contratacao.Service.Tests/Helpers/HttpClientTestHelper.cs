using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace Contratacao.Service.Tests.Helpers;

public static class HttpClientTestHelper
{
    public static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, object? responseContent)
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        var responseMessage = new HttpResponseMessage(statusCode);
        
        if (responseContent != null)
        {
            var json = JsonSerializer.Serialize(responseContent);
            responseMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        return new HttpClient(mockHandler.Object);
    }

    public static HttpClient CreateMockHttpClientWithException(Exception exception)
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        return new HttpClient(mockHandler.Object);
    }
}