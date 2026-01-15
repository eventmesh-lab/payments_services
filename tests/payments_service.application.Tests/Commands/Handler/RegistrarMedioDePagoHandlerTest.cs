using Moq;
using Xunit;
using System.Net;
using payments_services.application.Commands.Handlers;
using payments_services.application.Commands.Commands;
using payments_services.application.DTOs;
using payments_services.application.Interfaces;
using payments_services.domain.Interfaces;

namespace payments_services.application.Tests.Handlers
{
    public class RegistrarMedioDePagoHandlerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IStripeService> _stripeServiceMock;
        private readonly Mock<INotificationServices> _notificationServicesMock;
        private readonly RegistrarMedioDePagoHandler _handler;

        public RegistrarMedioDePagoHandlerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _stripeServiceMock = new Mock<IStripeService>();
            _notificationServicesMock = new Mock<INotificationServices>();

            _handler = new RegistrarMedioDePagoHandler(
                _usuarioServiceMock.Object,
                _stripeServiceMock.Object,
                _notificationServicesMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenUserIsNewAndRegistrationIsSuccessful()
        {
            var correo = "nuevo@test.com";
            var idUsuario = Guid.NewGuid();
            var idStripe = "cus_new123";
            var request = CreateRequest(correo);

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(idUsuario);

            _stripeServiceMock.Setup(s => s.ExisteClienteConEmailAsync(correo))
                .ReturnsAsync(false); 

            _stripeServiceMock.Setup(s => s.CrearUsuarioStripeAsync(correo))
                .ReturnsAsync(idStripe);

            _stripeServiceMock.Setup(s => s.AgregarMedioDePagoStripeAsync(idStripe, request.medioPagoDTO.medioPagoStripeID))
                .ReturnsAsync(HttpStatusCode.OK);

            var result = await _handler.Handle(request, CancellationToken.None);

            
            Assert.True(result);
            _stripeServiceMock.Verify(s => s.CrearUsuarioStripeAsync(correo), Times.Once);
            _stripeServiceMock.Verify(s => s.ObtenerUsuarioStripeAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenUserExistsAndRegistrationIsSuccessful()
        {
            var correo = "existente@test.com";
            var idUsuario = Guid.NewGuid();
            var idStripe = "cus_exist123";
            var request = CreateRequest(correo);

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(idUsuario);

            _stripeServiceMock.Setup(s => s.ExisteClienteConEmailAsync(correo))
                .ReturnsAsync(true); 

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(idUsuario))
                .ReturnsAsync(idStripe);

            _stripeServiceMock.Setup(s => s.AgregarMedioDePagoStripeAsync(idStripe, request.medioPagoDTO.medioPagoStripeID))
                .ReturnsAsync(HttpStatusCode.OK);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result);
            _stripeServiceMock.Verify(s => s.ObtenerUsuarioStripeAsync(idUsuario), Times.Once);
            _stripeServiceMock.Verify(s => s.CrearUsuarioStripeAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserNotFoundInLocalDatabase()
        {
            var request = CreateRequest("noexiste@test.com");
            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Contains("Ha ocurrido un error al registrar el medio de pago en la stripe", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenStripeRegistrationFails()
        {
            var correo = "test@test.com";
            var request = CreateRequest(correo);

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(Guid.NewGuid());

            _stripeServiceMock.Setup(s => s.ExisteClienteConEmailAsync(correo))
                .ReturnsAsync(true);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(It.IsAny<Guid>()))
                .ReturnsAsync("cus_123");

            _stripeServiceMock.Setup(s => s.AgregarMedioDePagoStripeAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(HttpStatusCode.BadRequest);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Contains("Ha ocurrido un error al registrar el medio de pago en la stripe", exception.Message);
            Assert.Contains("Fallo al registrar medio de pago", exception.InnerException.Message);
        }

        private RegistrarMedioPagoCommand CreateRequest(string correo)
        {
            return new RegistrarMedioPagoCommand(new RegistrarMedioDePagoDTO
            {
                correo = correo,
                medioPagoStripeID = "pm_card_visa"
            });
        }
    }
}