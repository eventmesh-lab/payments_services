using Xunit;
using payments_services.application.Commands.Commands;
using payments_services.application.DTOs;

namespace payments_services.application.Tests.Commands
{
    public class RegistrarPagoCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            var dto = new RegistrarPagoDTO
            {
                correo = "usuario@test.com",
                monto = 5000,
                moneda = "usd"
            };

            var command = new RegistrarPagoCommand(dto);

            Assert.NotNull(command.medioDePagoDTO);
            Assert.Equal(dto, command.medioDePagoDTO);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var initialDto = new RegistrarPagoDTO { correo = "test@test.com" };
            var command = new RegistrarPagoCommand(initialDto);

            var newDto = new RegistrarPagoDTO { correo = "nuevo@test.com" };
            command.medioDePagoDTO = newDto;

            Assert.Equal(newDto, command.medioDePagoDTO);
        }
    }
}