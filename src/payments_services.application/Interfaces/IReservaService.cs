using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Interfaces
{
    public interface IReservaService
    {
        Task<Reserva> ObtenerReservaPorGuid(Guid idReserva);
    }
}
