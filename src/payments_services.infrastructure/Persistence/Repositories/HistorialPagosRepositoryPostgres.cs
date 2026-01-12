using Microsoft.EntityFrameworkCore;
using payments_services.domain.Entities;
using payments_services.domain.Interfaces;
using payments_services.infrastructure.Mappers;
using payments_services.infrastructure.Persistence.Context;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.infrastructure.Persistence.Repositories
{
    public class HistorialPagosRepositoryPostgres : IHistorialPagosRepositoryPostgres
    {
        /// <summary>
        /// Atributo que corresponde al contexto de la base de datos del Microservicio Pagos en PostgreSQL.
        /// </summary>
        private readonly AppDbContext _dbContext;

        public HistorialPagosRepositoryPostgres(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Metodo que se encarga de registrar el historial de pagos en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="historialPagos">Objeto que contiene los datos del historial de pagos a registrar.</param>
        /// <returns>Retorna un estado HTTP si la operación fue exitosa</returns>
        public async Task<Guid> RegistrarHistorialPagosAsync(HistorialPagos historialPagos)
        {
            var historialPagosBD = historialPagos.ToPostgres();
            await _dbContext.HistorialPagos.AddAsync(historialPagosBD);
            await _dbContext.SaveChangesAsync();
            return historialPagosBD.Id;
        }

        /// Obtiene una historia de pago por su reserva.
        public async Task<HistorialPagos?> GetHistoriaDePagoByReserva(Guid idReserva, CancellationToken cancellationToken)
        {
            var historialPagosModel = await _dbContext.HistorialPagos
                .FirstOrDefaultAsync(u => u.IdReserva == idReserva, cancellationToken);
            if (historialPagosModel == null)
            {
                return null;
            }
            return HistorialPagosPostgreSQLMapper.ToDomain(historialPagosModel);
        }

        /// Obtiene todos los pagos de un usuario existentes en el repositorio.
        public async Task<List<HistorialPagos>> GetHistorialPagosByUserAsync( Guid userId, CancellationToken cancellationToken)
        {
            var historialesPagosModel = await _dbContext.HistorialPagos
                .Where(h => h.IdUsuario == userId) 
                .ToListAsync(cancellationToken);

            var historialesPagos = historialesPagosModel
                .Select(HistorialPagosPostgreSQLMapper.ToDomain)
                .ToList();

            return historialesPagos;
        }

        /// Obtiene una historia de pago por su reserva.
        public async Task<bool> ExistePago(Guid idReserva, CancellationToken cancellationToken)
        {
            var historialPagosModel = await _dbContext.HistorialPagos
                .FirstOrDefaultAsync(u => u.IdReserva == idReserva, cancellationToken);
            if (historialPagosModel == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<HistorialPagos>> GetAllPagosAsync(CancellationToken cancellationToken)
        {
            var historialPagosModels = await _dbContext.HistorialPagos.ToListAsync(cancellationToken);

            var historialPagos = historialPagosModels
                .Select(HistorialPagosPostgreSQLMapper.ToDomain)
                .ToList();

            return historialPagos;
        }
    }
}
