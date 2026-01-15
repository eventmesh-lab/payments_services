using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using payments_services.infrastructure.Services;
using payments_services.application.DTOs;
using payments_services.domain.Entities;
using Xunit;

namespace payments_services.tests.Infrastructure
{
    public class ReservaServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly ReservaService _service;

        public ReservaServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new ReservaService(_httpClient);
        }

        [Fact]
        public async Task ObtenerReservaPorGuid_ShouldReturnReserva_WhenResponseIsSuccessful()
        {
            var idReserva = Guid.NewGuid();
            var dto = new ReservaDto
            {
                Id = idReserva,
                idUsuario = Guid.NewGuid(),
                montoTotal = 150.00m,
                IdEvento = Guid.NewGuid()
            };

            var jsonResponse = JsonSerializer.Serialize(dto);

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var result = await _service.ObtenerReservaPorGuid(idReserva);

            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.montoTotal, result.MontoTotal);
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains(idReserva.ToString())),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ObtenerReservaPorGuid_ShouldReturnNull_WhenResponseIsNotFound()
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
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _service.ObtenerReservaPorGuid(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerReservaPorGuid_ShouldReturnNull_WhenJsonIsInvalid()
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
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("invalid-json")
                });

            var result = await _service.ObtenerReservaPorGuid(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerReservaPorGuid_ShouldReturnNull_WhenExceptionOccurs()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network fail"));

            var result = await _service.ObtenerReservaPorGuid(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}