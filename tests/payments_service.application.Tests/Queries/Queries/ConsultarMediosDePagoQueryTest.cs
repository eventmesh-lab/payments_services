using Xunit;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;
using System.Collections.Generic;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarMediosDePagoQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorreoProperty()
        {
            var correoEsperado = "usuario@test.com";

            var query = new ConsultarMediosDePagoQuery(correoEsperado);

            Assert.Equal(correoEsperado, query.correo);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var query = new ConsultarMediosDePagoQuery("inicial@test.com");
            var nuevoCorreo = "actualizado@test.com";

            query.correo = nuevoCorreo;

            Assert.Equal(nuevoCorreo, query.correo);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectListType()
        {
            var query = new ConsultarMediosDePagoQuery("test@test.com");

            Assert.IsAssignableFrom<MediatR.IRequest<List<MedioDePagoDTO>>>(query);
        }
    }
}