using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    public class EnviarCorreoPagoExitosoDto
    {
        public string Destinatario { get; set; }
        public string MontoPago { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
