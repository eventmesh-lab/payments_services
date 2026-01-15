using Moq;
using Moq.Protected;
using System.Net;
using payments_services.infrastructure.Services;
using Xunit;

namespace payments_services.tests.Infrastructure
{
    public class CouponServicesTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly CouponServices _service;

        public CouponServicesTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new CouponServices(_httpClient);
        }

        [Fact]
        public async Task MarcarCuponComoUsado_ShouldSendPutRequest_WhenSuccessful()
        {
            var couponId = Guid.NewGuid();
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

            await _service.MarcarCuponComoUsado(couponId);

            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put &&
                    req.RequestUri.ToString().Contains(couponId.ToString())),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task MarcarCuponComoUsado_ShouldThrowArgumentException_WhenResponseIsNotSuccessful()
        {
            var couponId = Guid.NewGuid();
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

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.MarcarCuponComoUsado(couponId));

            Assert.Equal("Error al actualizar el cupon", exception.Message);
        }

        [Fact]
        public async Task MarcarCuponComoUsado_ShouldRethrowException_WhenHttpClientFails()
        {
            var couponId = Guid.NewGuid();
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _service.MarcarCuponComoUsado(couponId));
        }
    }
}