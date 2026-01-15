using Moq;
using Xunit;
using MassTransit;
using Stripe;
using System.Text.Json;
using payments_services.application.Commands.Handlers;
using payments_services.application.Commands.Commands;
using payments_services.application.DTOs;
using payments_services.application.Interfaces;
using payments_services.domain.Interfaces;
using payments_services.domain.Entities;
using Events.Shared;
using payments_services.domain.ValueObjects;

namespace payments_services.application.Tests.Handlers
{
    public class RegistrarPagoHandlerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IStripeService> _stripeServiceMock;
        private readonly Mock<IHistorialPagosRepositoryPostgres> _historialRepoMock;
        private readonly Mock<INotificationServices> _notificacionServiceMock;
        private readonly Mock<IActivityService> _activityServiceMock;
        private readonly Mock<ICouponServices> _couponServicesMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly RegistrarPagoHandler _handler;

        public RegistrarPagoHandlerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _stripeServiceMock = new Mock<IStripeService>();
            _historialRepoMock = new Mock<IHistorialPagosRepositoryPostgres>();
            _notificacionServiceMock = new Mock<INotificationServices>();
            _activityServiceMock = new Mock<IActivityService>();
            _couponServicesMock = new Mock<ICouponServices>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _reservaServiceMock = new Mock<IReservaService>();

            _handler = new RegistrarPagoHandler(
                _usuarioServiceMock.Object,
                _stripeServiceMock.Object,
                _historialRepoMock.Object,
                _notificacionServiceMock.Object,
                _activityServiceMock.Object,
                _reservaServiceMock.Object,
                _couponServicesMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenPaymentProcessIsSuccessful()
        {
            var request = CreateValidRequest();
            var idUsuario = Guid.NewGuid();
            var idStripe = "cus_test123";
            var paymentIntent = new PaymentIntent { Status = "succeeded" };
            var medioPago = new MedioDePago(idUsuario, idStripe, "pm_test", "4242", new TipoPagoMedioPagoVO( "visa"), new MedioPredeterminadoMedioPagoVO(false));

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(request.medioDePagoDTO.correo))
                .ReturnsAsync(idUsuario);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(idUsuario))
                .ReturnsAsync(idStripe);

            _stripeServiceMock.Setup(s => s.RegistrarPagoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(paymentIntent);

            _stripeServiceMock.Setup(s => s.ObtenerMedioDePagoStripeAsync(idStripe, request.medioDePagoDTO.stripeMedioPagoId))
                .ReturnsAsync(medioPago);

            _historialRepoMock.Setup(s => s.RegistrarHistorialPagosAsync(It.IsAny<HistorialPagos>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result);
            _notificacionServiceMock.Verify(s => s.EnviarPagoExitoso(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _couponServicesMock.Verify(s => s.MarcarCuponComoUsado(request.medioDePagoDTO.idCoupon), Times.Once);
            _publishEndpointMock.Verify(p => p.Publish<IAuditLogCreated>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
        {
            var request = CreateValidRequest();
            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal("El usuario no existe en la base de datos.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenPaymentFailsInStripe()
        {
            var request = CreateValidRequest();
            var paymentIntent = new PaymentIntent { Status = "failed" };

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.NewGuid());

            _stripeServiceMock.Setup(s => s.RegistrarPagoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(paymentIntent);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal("No se ha podido realizar el pago de la reserva.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldRetry_WhenStripeServiceThrowsException()
        {
            var request = CreateValidRequest();
            var idUsuario = Guid.NewGuid();

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(idUsuario);

            _stripeServiceMock.SetupSequence(s => s.RegistrarPagoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()))
                .ThrowsAsync(new Exception("Error temporal de Stripe"))
                .ThrowsAsync(new Exception("Error temporal de Stripe"))
                .ReturnsAsync(new PaymentIntent { Status = "succeeded" });

            _stripeServiceMock.Setup(s => s.ObtenerMedioDePagoStripeAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new MedioDePago(idUsuario, "cus_123", "pm_123", "1234", new TipoPagoMedioPagoVO("visa"), new MedioPredeterminadoMedioPagoVO(true)));
            _historialRepoMock.Setup(s => s.RegistrarHistorialPagosAsync(It.IsAny<HistorialPagos>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result);
            _stripeServiceMock.Verify(s => s.RegistrarPagoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()), Times.Exactly(3));
        }

        private RegistrarPagoCommand CreateValidRequest()
        {
            return new RegistrarPagoCommand(new RegistrarPagoDTO
            {
                correo = "test@user.com",
                monto = 100,
                moneda = "usd",
                stripeMedioPagoId = "pm_test",
                idEvento = Guid.NewGuid(),
                idCoupon = Guid.NewGuid()
            });
        }
    }
}