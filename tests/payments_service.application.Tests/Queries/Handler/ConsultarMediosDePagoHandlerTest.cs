using Moq;
using Xunit;
using payments_services.application.Queries.Handlers;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;
using payments_services.domain.Interfaces;
using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using payments_services.domain.ValueObjects;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarMediosDePagoHandlerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IStripeService> _stripeServiceMock;
        private readonly ConsultarMediosDePagoHandler _handler;

        public ConsultarMediosDePagoHandlerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _stripeServiceMock = new Mock<IStripeService>();
            _handler = new ConsultarMediosDePagoHandler(_usuarioServiceMock.Object, _stripeServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnListMedioDePagoDTO_WhenSuccessful()
        {
            var correo = "usuario@test.com";
            var userId = Guid.NewGuid();
            var stripeUserId = "cus_abc123";
            var query = new ConsultarMediosDePagoQuery(correo);

            var mediosDePagoDomain = new List<MedioDePago>
            {
                new MedioDePago(userId, stripeUserId, "pm_1", "1234", new TipoPagoMedioPagoVO("visa"), new MedioPredeterminadoMedioPagoVO(true)),
                new MedioDePago(userId, stripeUserId, "pm_2", "5678", new TipoPagoMedioPagoVO("mastercard"), new MedioPredeterminadoMedioPagoVO(true))
            };

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(userId);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(userId))
                .ReturnsAsync(stripeUserId);

            _stripeServiceMock.Setup(s => s.ObtenerMediosDePagoStripeAsync(stripeUserId))
                .ReturnsAsync(mediosDePagoDomain);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("pm_1", result[0].idMedioPago);
            Assert.True(result[0].medioPredeterminado);
            Assert.Equal("1234", result[0].ultimosCuatroDigitos);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenUserNotFound()
        {
            var query = new ConsultarMediosDePagoQuery("inexistente@test.com");
            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener los medio de pago en stripe", exception.Message);
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("El usuario no existe en la base de datos", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenNoPaymentMethodsFound()
        {
            var correo = "sinpagos@test.com";
            var userId = Guid.NewGuid();
            var stripeUserId = "cus_empty";
            var query = new ConsultarMediosDePagoQuery(correo);

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(userId);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(userId))
                .ReturnsAsync(stripeUserId);

            _stripeServiceMock.Setup(s => s.ObtenerMediosDePagoStripeAsync(stripeUserId))
                .ReturnsAsync(new List<MedioDePago>());

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("No existen medios de pago asociados al usuario.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Handle_ShouldWrapStripeServiceExceptions()
        {
            var correo = "error@test.com";
            var userId = Guid.NewGuid();
            var query = new ConsultarMediosDePagoQuery(correo);

            _usuarioServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(correo))
                .ReturnsAsync(userId);

            _stripeServiceMock.Setup(s => s.ObtenerUsuarioStripeAsync(userId))
                .ThrowsAsync(new Exception("Error de API Stripe"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Ha ocurrido un error al obtener los medio de pago en stripe", exception.Message);
            Assert.Equal("Error de API Stripe", exception.InnerException.Message);
        }
    }
}