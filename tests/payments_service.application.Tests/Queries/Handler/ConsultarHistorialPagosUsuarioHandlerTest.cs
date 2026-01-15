using Moq;
using Xunit;
using payments_services.application.Queries.Handlers;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;
using payments_services.application.Interfaces;
using payments_services.domain.Interfaces;
using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarHistorialPagosUsuarioHandlerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IHistorialPagosRepositoryPostgres> _historialRepoMock;
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ConsultarHistorialPagosUsuarioHandler _handler;

        public ConsultarHistorialPagosUsuarioHandlerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _historialRepoMock = new Mock<IHistorialPagosRepositoryPostgres>();
            _reservaServiceMock = new Mock<IReservaService>();

            _handler = new ConsultarHistorialPagosUsuarioHandler(
                _usuarioServiceMock.Object,
                _historialRepoMock.Object,
                _reservaServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnList_WhenHistoryExists()
        {
            var correo = "test@user.com";
            var userId = Guid.NewGuid();
            var query = new ConsultarHistorialPagosUsuarioQuery(correo);

            var historialEntities = new List<HistorialPagos>
            {
                new HistorialPagos {
                    Id = Guid.NewGuid(),
                    IdUsuario = userId,
                    IdEvento = Guid.NewGuid(),
                    MontoPago =new MontoHistorialPagosVO(200),
                    CreatedAt = DateTime.UtcNow,
                    UltimosDigitosTarjeta = "1234",
                    TipoMedioDePago = "Visa"
                }
            };

            var mockUsuario = new Usuario
            {
                Nombre = "Juan Perez"
            };

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(userId);

            _historialRepoMock.Setup(s => s.GetHistorialPagosByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(historialEntities);

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmail(correo))
                .ReturnsAsync(mockUsuario);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Visa", result[0].MetododePago);
            Assert.Equal("Juan Perez", result[0].NombreUsuario);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenUserNotFound()
        {
            var query = new ConsultarHistorialPagosUsuarioQuery("noexiste@test.com");
            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener el historial de pago en la bd", exception.Message);
            Assert.Equal("El usuario no existe en la base de datos.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenHistoryIsEmpty()
        {
            var userId = Guid.NewGuid();
            var query = new ConsultarHistorialPagosUsuarioQuery("user@test.com");

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(userId);

            _historialRepoMock.Setup(s => s.GetHistorialPagosByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<HistorialPagos>());

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("El usuario no posee pagos asociados.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldWrapGeneralException()
        {
            var query = new ConsultarHistorialPagosUsuarioQuery("error@test.com");
            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB Error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener el historial de pago en la bd", exception.Message);
            Assert.Equal("DB Error", exception.InnerException.Message);
        }
    }
}