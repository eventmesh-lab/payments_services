using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    public class ConsultarMediosDePagoDTO
    {
        /// <summary>
        /// Atributo que corresponde al correo del usuario al que le pertenece el medio de pago a consultar.
        /// </summary>
        public string correo { get; set; }
        /// <summary>
        /// Atributo que corresponde al medio de pago a consultar.
        /// </summary>
        public string idMedioDePagoStripe { get; set; }
    }
}
