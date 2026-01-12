using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Factory
{
    public static class HistorialPagosFactory
    {
        public static HistorialPagos CrearHistorialPagos(Guid idusuario, Guid idReserva, string idMedioPago, decimal montoPago, string ultimosdigitos, string tipoMedioDePago)
        {
            var montoPagoVO = new MontoHistorialPagosVO(montoPago);
            return new HistorialPagos(idusuario, idReserva, idMedioPago, montoPagoVO, ultimosdigitos, tipoMedioDePago);
        }
    }
}
