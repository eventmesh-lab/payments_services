using payments_services.domain.Entities;
using payments_services.domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Factory
{
    public static class MedioDePagoFactory
    {
        public static MedioDePago CrearMedioDePago(Guid idusuario, string stripeclientid, string stripemediopagoid, string ultimosdigitos, string tipopago, bool mediopredeterminado)
        {
            var tipoMedioPagoVO = new TipoPagoMedioPagoVO(tipopago);
            var medioPredeterminadoMedioPagoVO = new MedioPredeterminadoMedioPagoVO(mediopredeterminado);
            return new MedioDePago(idusuario, stripeclientid, stripemediopagoid, ultimosdigitos, tipoMedioPagoVO, medioPredeterminadoMedioPagoVO);
        }
    }
}
