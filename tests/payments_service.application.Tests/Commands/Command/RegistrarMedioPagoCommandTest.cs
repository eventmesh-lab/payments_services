using Xunit;
using payments_services.application.Commands.Commands;
using payments_services.application.DTOs;

namespace payments_services.application.Tests.Commands
{
    public class RegistrarMedioPagoCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            var dto = new RegistrarMedioDePagoDTO
            {
                correo = "test@example.com",
                medioPagoStripeID = "pm_12345"
            };

            var command = new RegistrarMedioPagoCommand(dto);

            Assert.NotNull(command.medioPagoDTO);
            Assert.Equal(dto, command.medioPagoDTO);
            Assert.Equal("test@example.com", command.medioPagoDTO.correo);
            Assert.Equal("pm_12345", command.medioPagoDTO.medioPagoStripeID);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var initialDto = new RegistrarMedioDePagoDTO { correo = "initial@test.com" };
            var command = new RegistrarMedioPagoCommand(initialDto);

            var newDto = new RegistrarMedioDePagoDTO { correo = "updated@test.com" };
            command.medioPagoDTO = newDto;

            Assert.Equal(newDto, command.medioPagoDTO);
            Assert.Equal("updated@test.com", command.medioPagoDTO.correo);
        }
    }
}