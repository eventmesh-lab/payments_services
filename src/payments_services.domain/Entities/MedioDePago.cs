using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.domain.ValueObjects;

namespace payments_services.domain.Entities
{
    /// <summary>
    /// Clase Entity que representa a la entidad Medio de Pago en el dominio del sistema.
    /// </summary>
    public class MedioDePago
    {
        /// <summary>
        /// Atributo que corresponde al ID del usuario perteneciente al medio de pago en el Microservicio Usuarios .
        /// </summary>
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Atributo que corresponde al ID del usuario perteneciente al medio de pago en Stripe .
        /// </summary>
        public string StripeClienteId { get; set; }

        /// <summary>
        /// Atributo que corresponde al ID del medio de pago .
        /// </summary>
        public string StripeMedioPagoId { get; set; }

        /// <summary>
        /// Atributo que corresponde a los ultimos cuatro digitos del medio de pago .
        /// </summary>
        public string UltimosDigitosTarjeta { get; set; }

        /// <summary>
        /// Atributo que corresponde al tipo de medio de pago .
        /// </summary>
        public TipoPagoMedioPagoVO TipoTarjeta { get; set; }

        /// <summary>
        /// Atributo que corresponde a determinar si el medio de pago es predeterminado (True, False) .
        /// </summary>
        public MedioPredeterminadoMedioPagoVO MedioPredeterminado { get; set; }

        public MedioDePago(Guid idusuario, string stripeclientid, string stripemediopagoid, string ultimosdigitos,
            TipoPagoMedioPagoVO tipopago, MedioPredeterminadoMedioPagoVO mediopredeterminado)
        {
            IdUsuario = idusuario;
            StripeClienteId = stripeclientid;
            StripeMedioPagoId = stripemediopagoid;
            UltimosDigitosTarjeta = ultimosdigitos;
            TipoTarjeta = tipopago;
            MedioPredeterminado = mediopredeterminado;
        }
    }
}