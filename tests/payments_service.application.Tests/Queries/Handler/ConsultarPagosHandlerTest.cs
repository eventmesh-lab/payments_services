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
    public class ConsultarPagosHandlerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IHistorialPagosRepositoryPostgres> _historialRepoMock;
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ConsultarPagosHandler _handler;

        public ConsultarPagosHandlerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _historialRepoMock = new Mock<IHistorialPagosRepositoryPostgres>();
            _reservaServiceMock = new Mock<IReservaService>();

            _handler = new ConsultarPagosHandler(
                _usuarioServiceMock.Object,
                _historialRepoMock.Object,
                _reservaServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnList_WhenPagosExist()
        {
            var query = new ConsultarPagosQuery();
            var userId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();

            var historialEntities = new List<HistorialPagos>
            {
                new HistorialPagos
                {
                    Id = Guid.NewGuid(),
                    IdUsuario = userId,
                    IdEvento = eventoId,
                    MontoPago = new MontoHistorialPagosVO(150),
                    CreatedAt = DateTime.UtcNow,
                    UltimosDigitosTarjeta = "9876",
                    TipoMedioDePago = "Mastercard"
                }
            };

            _historialRepoMock.Setup(s => s.GetAllPagosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(historialEntities);

            _reservaServiceMock.Setup(s => s.ObtenerReservaPorGuid(eventoId))
                .ReturnsAsync(new Reserva(userId, 150m, eventoId));

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorId(userId))
                .ReturnsAsync(new Usuario("Angel Blanco", "tahmaris18@gmail.com"));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Angel Blanco", result[0].NombreUsuario);
            Assert.Equal("Mastercard", result[0].MetododePago);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenNoPagosFound()
        {
            var query = new ConsultarPagosQuery();

            _historialRepoMock.Setup(s => s.GetAllPagosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<HistorialPagos>());

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener el historial de pago en la bd", exception.Message);
            Assert.Equal("No hay pagos registrados.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenRepositoryFails()
        {
            var query = new ConsultarPagosQuery();

            _historialRepoMock.Setup(s => s.GetAllPagosAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener el historial de pago en la bd", exception.Message);
            Assert.Equal("Database connection failed", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnList_WhenMultiplePagosExist()
        {
            var query = new ConsultarPagosQuery();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var eventoId1 = Guid.NewGuid();
            var eventoId2 = Guid.NewGuid();

            var historialEntities = new List<HistorialPagos>
            {
                new HistorialPagos { IdUsuario = id1, IdEvento = eventoId1, MontoPago = new MontoHistorialPagosVO(10) },
                new HistorialPagos { IdUsuario = id2, IdEvento = eventoId2, MontoPago = new MontoHistorialPagosVO(20) }
            };

            _historialRepoMock.Setup(s => s.GetAllPagosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(historialEntities);

            _reservaServiceMock.Setup(s => s.ObtenerReservaPorGuid(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Reserva(Guid.NewGuid(), 100m, id));

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorId(It.IsAny<Guid>()))
                .ReturnsAsync(new Usuario
                {
                    Nombre = "Usuario Test"
                });

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal("Usuario Test", r.NombreUsuario));
        }
    }
}