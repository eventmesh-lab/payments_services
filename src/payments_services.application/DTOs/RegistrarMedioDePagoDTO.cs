using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    /// <summary>
    /// Clase DTO que se encarga de encapsular la información necesaria para registrar un medio de pago.
    /// </summary>
    public class RegistrarMedioDePagoDTO
    {
        /// <summary>
        /// Atributo que contiene el token generado por Stripe con los datos del medio de pago.
        /// </summary>
        public string medioPagoStripeID { get; set; }
        /// <summary>
        /// Atributo que contiene el correo del usuario que registra el medio de pago.
        /// </summary>
        public string correo { get; set; }

    }
}
