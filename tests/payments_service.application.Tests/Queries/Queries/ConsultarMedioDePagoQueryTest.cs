using Xunit;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarMedioDePagoQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeMedioDePagoDTOProperty()
        {
            var dto = new ConsultarMediosDePagoDTO
            {
                correo = "test@example.com",
                idMedioDePagoStripe = "pm_12345"
            };

            var query = new ConsultarMedioDePagoQuery(dto);

            Assert.NotNull(query.medioDePagoDTO);
            Assert.Equal(dto, query.medioDePagoDTO);
            Assert.Equal("test@example.com", query.medioDePagoDTO.correo);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var initialDto = new ConsultarMediosDePagoDTO { correo = "inicial@test.com" };
            var query = new ConsultarMedioDePagoQuery(initialDto);
            var newDto = new ConsultarMediosDePagoDTO { correo = "nuevo@test.com" };

            query.medioDePagoDTO = newDto;

            Assert.Equal(newDto, query.medioDePagoDTO);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectType()
        {
            var dto = new ConsultarMediosDePagoDTO();
            var query = new ConsultarMedioDePagoQuery(dto);

            Assert.IsAssignableFrom<MediatR.IRequest<MedioDePagoDTO>>(query);
        }
    }
}