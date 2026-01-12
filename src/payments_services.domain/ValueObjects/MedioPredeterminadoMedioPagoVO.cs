using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.ValueObjects
{
    public class MedioPredeterminadoMedioPagoVO
    {
        public bool medioPredeterminado { get; set; }

        public MedioPredeterminadoMedioPagoVO(bool mediopredeterminado)
        {
            medioPredeterminado = mediopredeterminado;
        }
    }
}
