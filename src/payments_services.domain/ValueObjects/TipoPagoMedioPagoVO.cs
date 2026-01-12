using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.ValueObjects
{
    public class TipoPagoMedioPagoVO
    {
        public string tipoPago { get; set; }

        public TipoPagoMedioPagoVO(string tipopago)
        {
            tipoPago = tipopago;
        }
    }
}
