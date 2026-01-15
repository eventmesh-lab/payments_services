using Xunit;
using payments_services.application.Queries.Queries;
using payments_services.application.DTOs;
using System.Collections.Generic;

namespace payments_services.application.Tests.Queries
{
    public class ConsultarPagosQueryTests
    {
        [Fact]
        public void Query_ShouldExistAndBeInitializable()
        {
            var query = new ConsultarPagosQuery();

            Assert.NotNull(query);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectListType()
        {
            var query = new ConsultarPagosQuery();

            Assert.IsAssignableFrom<MediatR.IRequest<List<HistorialPagosDTO>>>(query);
        }
    }
}