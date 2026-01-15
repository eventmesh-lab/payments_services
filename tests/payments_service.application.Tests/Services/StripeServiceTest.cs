using Moq;
using Xunit;
using Stripe;
using System.Net;
using payments_services.infrastructure.ExternalServices.Stripe.Services;
using payments_services.application.Interfaces;
using payments_services.domain.Entities;
using payments_services.domain.Interfaces;

namespace payments_services.tests.Infrastructure
{
    public class StripeServiceTests
    {
        private readonly Mock<IStripeCustomerService> _customerServiceMock;
        private readonly Mock<IStripePaymentMethodService> _paymentMethodMock;
        private readonly Mock<IStripePaymentIntentService> _paymentIntentMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly StripeService _stripeService;

        public StripeServiceTests()
        {
            _customerServiceMock = new Mock<IStripeCustomerService>();
            _paymentMethodMock = new Mock<IStripePaymentMethodService>();
            _paymentIntentMock = new Mock<IStripePaymentIntentService>();
            _usuarioServiceMock = new Mock<IUsuarioService>();

            _stripeService = new StripeService(
                _customerServiceMock.Object,
                _paymentMethodMock.Object,
                _usuarioServiceMock.Object,
                _paymentIntentMock.Object
            );
        }

        [Fact]
        public async Task CrearUsuarioStripeAsync_ShouldReturnStripeId()
        {
            var correo = "test@test.com";
            var userId = Guid.NewGuid();
            var customer = new Customer { Id = "cus_123" };

            _customerServiceMock.Setup(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>()))
                .ReturnsAsync(new Customer { Id = "cus_123" });

            _customerServiceMock.Setup(s => s.ListAsync(It.IsAny<CustomerListOptions>()))
                .ReturnsAsync(new StripeList<Customer> { Data = new List<Customer>() });

            var result = await _stripeService.CrearUsuarioStripeAsync(correo);

            Assert.Equal("cus_123", result);
        }

        [Fact]
        public async Task ExisteClienteConEmailAsync_ShouldReturnTrue_WhenExists()
        {
            var email = "existe@test.com";
            var customers = new StripeList<Customer>
            {
                Data = new List<Customer> { new Customer { Email = email } }
            };

            _customerServiceMock.Setup(s => s.ListAsync(It.IsAny<CustomerListOptions>()))
                .ReturnsAsync(customers);

            var result = await _stripeService.ExisteClienteConEmailAsync(email);

            Assert.True(result);
        }

        [Fact]
        public async Task ObtenerUsuarioStripeAsync_ShouldReturnId_WhenMetadataMatches()
        {
            var userId = Guid.NewGuid();
            var customers = new StripeList<Customer>
            {
                Data = new List<Customer>
                {
                    new Customer
                    {
                        Id = "cus_abc",
                        Metadata = new Dictionary<string, string> { { "usuarioId", userId.ToString() } }
                    }
                }
            };

            _customerServiceMock.Setup(s => s.ListAsync(It.IsAny<CustomerListOptions>()))
                .ReturnsAsync(customers);

            var result = await _stripeService.ObtenerUsuarioStripeAsync(userId);

            Assert.Equal("cus_abc", result);
        }

        [Fact]
        public async Task AgregarMedioDePagoStripeAsync_ShouldReturnOk()
        {
            _paymentMethodMock.Setup(s => s.AttachAsync(It.IsAny<string>(), It.IsAny<PaymentMethodAttachOptions>()))
                .ReturnsAsync(new PaymentMethod());

            var result = await _stripeService.AgregarMedioDePagoStripeAsync("cus_1", "pm_1");

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task EstablecerMedioDePagoPredeterminadoStripeAsync_ShouldReturnOk()
        {
            _customerServiceMock.Setup(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateOptions>()))
                .ReturnsAsync(new Customer());

            var result = await _stripeService.EstablecerMedioDePagoPredeterminadoStripeAsync("cus_1", "pm_1");

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task EliminarMedioDePagoStripeAsync_ShouldReturnOk()
        {
            _paymentMethodMock.Setup(s => s.DetachAsync(It.IsAny<string>()))
                .ReturnsAsync(new PaymentMethod());

            var result = await _stripeService.EliminarMedioDePagoStripeAsync("pm_1");

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task RegistrarPagoAsync_ShouldReturnPaymentIntent()
        {
            var expectedIntent = new PaymentIntent { Id = "pi_123", Status = "succeeded" };

            _customerServiceMock.Setup(s => s.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new Customer());

            _paymentIntentMock.Setup(s => s.CreateAsync(It.IsAny<PaymentIntentCreateOptions>()))
                .ReturnsAsync(expectedIntent);

            var result = await _stripeService.RegistrarPagoAsync("cus_1", "pm_1", Guid.NewGuid(), Guid.NewGuid(), "usd", 1000);

            Assert.NotNull(result);
            Assert.Equal("pi_123", result.Id);
        }

        [Fact]
        public async Task ObtenerIdClientePorEmailAsync_ShouldReturnId()
        {
            var email = "test@test.com";
            var customers = new StripeList<Customer>
            {
                Data = new List<Customer> { new Customer { Id = "cus_999", Email = email } }
            };

            _customerServiceMock.Setup(s => s.ListAsync(It.IsAny<CustomerListOptions>()))
                .ReturnsAsync(customers);

            var result = await _stripeService.ObtenerIdClientePorEmailAsync(email);

            Assert.Equal("cus_999", result);
        }
    }
}