using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Interfaces
{
    public interface IHistorialPagosRepositoryPostgres
    {
        Task<Guid> RegistrarHistorialPagosAsync(HistorialPagos historialPagos);
        Task<HistorialPagos?> GetHistoriaDePagoByReserva(Guid idReserva, CancellationToken cancellationToken);
        Task<List<HistorialPagos>> GetHistorialPagosByUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistePago(Guid idReserva, CancellationToken cancellationToken);
        Task<List<HistorialPagos>> GetAllPagosAsync(CancellationToken cancellationToken);
    }
}
