using Xunit;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;
using System.Collections.Generic;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarHistorialPagosUsuarioQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorreoProperty()
        {
            var correoEsperado = "usuario@test.com";

            var query = new ConsultarHistorialPagosUsuarioQuery(correoEsperado);

            Assert.Equal(correoEsperado, query.correo);
        }

        [Fact]
        public void Query_ShouldAllowPropertyUpdates()
        {
            var query = new ConsultarHistorialPagosUsuarioQuery("inicial@test.com");
            var nuevoCorreo = "nuevo@test.com";

            query.correo = nuevoCorreo;

            Assert.Equal(nuevoCorreo, query.correo);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectType()
        {
            var query = new ConsultarHistorialPagosUsuarioQuery("test@test.com");

            Assert.IsAssignableFrom<MediatR.IRequest<List<HistorialPagosDTO>>>(query);
        }
    }
}