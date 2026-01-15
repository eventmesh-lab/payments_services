using Moq;
using Moq.Protected;
using System.Net;
using payments_services.infrastructure.Services;
using payments_services.application.DTOs;
using Xunit;

namespace payments_services.tests.Infrastructure
{
    public class ActivityServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly ActivityService _service;

        public ActivityServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _service = new ActivityService(_httpClient);
        }

        [Fact]
        public async Task RegisterActivityAsync_ShouldReturnTrue_WhenResponseIsSuccessful()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var result = await _service.RegisterActivityAsync("test@test.com", "Accion", "Categoria");

            Assert.True(result);
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("test@test.com")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task RegisterActivityAsync_ShouldReturnFalse_WhenResponseIsError()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var result = await _service.RegisterActivityAsync("test@test.com", "Accion", "Categoria");

            Assert.False(result);
        }

        [Fact]
        public async Task RegisterActivityAsync_ShouldReturnFalse_WhenExceptionOccurs()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Connection Failed"));

            var result = await _service.RegisterActivityAsync("test@test.com", "Accion", "Categoria");

            Assert.False(result);
        }
    }
}