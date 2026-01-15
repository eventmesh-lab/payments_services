using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    public class RegistrarPagoDTO
    {
        /// <summary>
        /// Atributo que corresponde al ID del medio de pago.
        /// </summary>
        public string stripeMedioPagoId { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID de la reserva.
        /// </summary>
        public Guid idEvento { get; set; }
        /// <summary>
        /// Atributo que contiene el correo del usuario que registra el pago.
        /// </summary>
        public string correo { get; set; }
        /// <summary>
        /// Atributo que contiene la moneda con la que se realiza el pago.
        /// </summary>
        public string moneda { get; set; }
        /// <summary>
        /// Atributo que contiene el monto del pago .
        /// </summary>
        public decimal monto { get; set; }
        public Guid idCoupon {get; set; }
    }
}
