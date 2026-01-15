using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using payments_services.infrastructure.Services;
using payments_services.application.DTOs;
using Xunit;

namespace payments_services.tests.Infrastructure
{
    public class NotificationServicesTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly NotificationServices _service;

        public NotificationServicesTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new NotificationServices(_httpClient);
        }

        [Fact]
        public async Task EnviarPagoExitoso_ShouldPostCorrectly_WhenSuccessful()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            await _service.EnviarPagoExitoso("user123", "100USD");

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().EndsWith("paymentSuccessNotification")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ReservaConfirmadaExitos_ShouldThrowArgumentException_WhenResponseIsError()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ReservaConfirmadaExitos("test@test.com"));

            Assert.Equal("Error al enviar la notificacion", exception.Message);
        }

        [Fact]
        public async Task EnviarCorreoPagoExitosoDetallado_ShouldSendAllData_WhenSuccessful()
        {
            var dto = new EnviarCorreoPagoExitosoDto
            {
                Destinatario = "test@test.com",
                MontoPago = "50USD",
                FechaPago = DateTime.UtcNow
            };

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            await _service.EnviarCorreoPagoExitosoDetallado(dto);

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("paymentSuccessNotificationEmail")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task EnviarCorreoPagoExitosoDetallado_ShouldThrowWithDetail_WhenFails()
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
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Server Error Detail")
                });

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.EnviarCorreoPagoExitosoDetallado(new EnviarCorreoPagoExitosoDto()));

            Assert.Contains("Error al enviar el correo de pago detallado", exception.Message);
            Assert.Contains("Server Error Detail", exception.Message);
        }

        [Fact]
        public async Task AnyMethod_ShouldRethrow_WhenSystemExceptionOccurs()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("No network"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _service.EnviarPagoExitoso("id", "amt"));
        }
    }
}