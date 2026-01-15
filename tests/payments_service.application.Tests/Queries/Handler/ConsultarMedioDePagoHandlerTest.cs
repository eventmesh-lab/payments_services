using Moq;
using Xunit;
using payments_services.application.Queries.Handlers;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;
using payments_services.domain.Interfaces;
using payments_services.domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using payments_services.domain.ValueObjects;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarMedioDePagoHandlerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IStripeService> _stripeServiceMock;
        private readonly ConsultarMedioDePagoHandler _handler;

        public ConsultarMedioDePagoHandlerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _stripeServiceMock = new Mock<IStripeService>();
            _handler = new ConsultarMedioDePagoHandler(_usuarioServiceMock.Object, _stripeServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMedioDePagoDTO_WhenSuccessful()
        {
            var correo = "test@example.com";
            var userId = Guid.NewGuid();
            var stripeUserId = "cus_12345";
            var stripePmId = "pm_67890";

            var request = new ConsultarMedioDePagoQuery(new ConsultarMediosDePagoDTO
            {
                correo = correo,
                idMedioDePagoStripe = stripePmId
            });

            var medioPagoDomain = new MedioDePago(
                userId,
                stripeUserId,
                stripePmId,
                "4242",
                new TipoPagoMedioPagoVO("visa"),
                new MedioPredeterminadoMedioPagoVO(true)
            );

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(userId);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(userId))
                .ReturnsAsync(stripeUserId);

            _stripeServiceMock.Setup(s => s.ObtenerMedioDePagoStripeAsync(stripeUserId, stripePmId))
                .ReturnsAsync(medioPagoDomain);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(stripePmId, result.idMedioPago);
            Assert.Equal("4242", result.ultimosCuatroDigitos);
            Assert.True(result.medioPredeterminado);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenUserDoesNotExist()
        {
            var request = new ConsultarMedioDePagoQuery(new ConsultarMediosDePagoDTO { correo = "noexiste@test.com" });

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener los medio de pago en la stripe", exception.Message);
            Assert.Equal("El usuario no existe en la base de datos.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenPaymentMethodNotFound()
        {
            var correo = "test@example.com";
            var userId = Guid.NewGuid();
            var stripeUserId = "cus_12345";
            var request = new ConsultarMedioDePagoQuery(new ConsultarMediosDePagoDTO
            {
                correo = correo,
                idMedioDePagoStripe = "pm_inexistente"
            });

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(userId);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(userId))
                .ReturnsAsync(stripeUserId);

            _stripeServiceMock.Setup(s => s.ObtenerMedioDePagoStripeAsync(stripeUserId, It.IsAny<string>()))
                .ReturnsAsync((MedioDePago)null);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal("No existen este medio de pago asociado al usuario.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldWrapGeneralExceptions()
        {
            var request = new ConsultarMedioDePagoQuery(new ConsultarMediosDePagoDTO { correo = "error@test.com" });

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error de conexión"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener los medio de pago en la stripe", exception.Message);
            Assert.Equal("Error de conexión", exception.InnerException.Message);
        }
    }
}