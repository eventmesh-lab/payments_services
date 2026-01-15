using payments_services.domain.Entities;
using payments_services.infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.infrastructure.Mappers
{
    /// <summary>
    /// Clase mapper que se encarga de mapear el objeto de tipo Entidad Historial Pagos (Dominio) a una entidad en la base de datos en PostgreSQL
    /// </summary>
    public static class HistorialPagosPostgreSQLMapper
    {
        /// <summary>
        /// Método que se encarga de mapear un producto (Entidad) a un modelo en la base de datos en PostgreSQL.
        /// </summary>
        /// <param name="historialPagos">Entidad que contiene los valores del historial pagos a registrar</param>
        /// <returns>Retorna un objeto de tipo HistorialPagosPostgreSQL, que corresponde al modelo de historial pagos en la base de datos en PostgreSQL.</returns>
        public static HistorialPagosPostgreSQL ToPostgres(this HistorialPagos historialPagos)
        {
            return new HistorialPagosPostgreSQL
            {
                Id = historialPagos.Id,
                IdUsuario = historialPagos.IdUsuario,
                IdEvento = historialPagos.IdEvento,
                IdMedioDePago = historialPagos.IdMedioDePago,
                Monto = historialPagos.MontoPago.montoPago,
                UltimosCuatroDigitos = historialPagos.UltimosDigitosTarjeta,
                CreatedAt = historialPagos.CreatedAt,
                TipoMedioDePago = historialPagos.TipoMedioDePago
            };
        }
        public static HistorialPagos ToDomain(this HistorialPagosPostgreSQL historialPagosPostgres)
        {
            return new HistorialPagos
            {
                Id = historialPagosPostgres.Id,
                IdUsuario = historialPagosPostgres.IdUsuario,
                IdEvento = historialPagosPostgres.IdEvento,
                IdMedioDePago = historialPagosPostgres.IdMedioDePago,
                MontoPago = new MontoHistorialPagosVO(historialPagosPostgres.Monto), // VO desde decimal
                UltimosDigitosTarjeta = historialPagosPostgres.UltimosCuatroDigitos,
                CreatedAt = historialPagosPostgres.CreatedAt,
                TipoMedioDePago = historialPagosPostgres.TipoMedioDePago
            };
        }

    }
}
