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
    public class UsuarioServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new UsuarioService(_httpClient);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmailAsync_ShouldReturnGuid_WhenResponseIsSuccessful()
        {
            var expectedGuid = Guid.NewGuid();
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
                    Content = new StringContent($"\"{expectedGuid}\"")
                });

            var result = await _service.ObtenerUsuarioPorEmailAsync("test@test.com");

            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmailAsync_ShouldReturnEmptyGuid_WhenResponseIsError()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            var result = await _service.ObtenerUsuarioPorEmailAsync("test@test.com");

            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmail_ShouldReturnUsuario_WhenSuccessful()
        {
            var dto = new UsuarioDto { Nombre = "Angel Blanco", Email = "angel@test.com" };
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

            var result = await _service.ObtenerUsuarioPorEmail("angel@test.com");

            Assert.NotNull(result);
            Assert.Equal("Angel Blanco", result.Nombre);
            Assert.Equal("angel@test.com", result.Email);
        }

        [Fact]
        public async Task ObtenerUsuarioPorId_ShouldReturnUsuario_WhenIdExistsInList()
        {
            var targetId = Guid.NewGuid();
            var userList = new List<UsuarioDto>
            {
                new UsuarioDto { Id = targetId, Nombre = "Target User", Email = "target@test.com" },
                new UsuarioDto { Id = Guid.NewGuid(), Nombre = "Other User", Email = "other@test.com" }
            };
            var jsonResponse = JsonSerializer.Serialize(userList);

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

            var result = await _service.ObtenerUsuarioPorId(targetId);

            Assert.NotNull(result);
            Assert.Equal("Target User", result.Nombre);
        }

        [Fact]
        public async Task ObtenerUsuarioPorId_ShouldReturnNull_WhenResponseIsError()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });

            var result = await _service.ObtenerUsuarioPorId(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmail_ShouldReturnNull_WhenExceptionOccurs()
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            var result = await _service.ObtenerUsuarioPorEmail("error@test.com");

            Assert.Null(result);
        }
    }
}