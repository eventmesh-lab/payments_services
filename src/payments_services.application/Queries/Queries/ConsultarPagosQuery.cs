using MediatR;
using payments_services.application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Queries.Queries
{
    public class ConsultarPagosQuery: IRequest<List<HistorialPagosDTO>>
    {
    }
}
