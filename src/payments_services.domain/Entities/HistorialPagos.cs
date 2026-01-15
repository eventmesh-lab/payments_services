using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace payments_services.domain.Entities
{
    /// <summary>
    /// Clase Entity que representa a la entidad historial de pagos en el dominio del sistema.
    /// </summary>
    public class HistorialPagos
    {
        /// <summary>
        /// Atributo que corresponde al ID del historial de pago .
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID del usuario perteneciente al pago realizado  .
        /// </summary>
        public Guid IdUsuario { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID del medio de pago con el que se realizó el pago  .
        /// </summary>
        public string IdMedioDePago { get; set; }
        public Guid IdEvento { get; set; }
        /// <summary>
        /// Atributo que corresponde al ID del medio de pago con el que se realizó el pago  .
        /// </summary>
        public string TipoMedioDePago { get; set; }
        /// <summary>
        /// Atributo que corresponde al monto del pago realizado
        /// </summary>
        public MontoHistorialPagosVO MontoPago { get; set; }
        /// <summary>
        /// Atributo que corresponde a la fecha en la que el pago fue realizado
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Atributo que corresponde a los ultimos cuatro digitos del medio de pago con el que el pago fue realizado
        /// </summary>
        public string UltimosDigitosTarjeta { get; set; }

        public HistorialPagos() { }

        [JsonConstructor]
        public HistorialPagos(Guid idUsuario, Guid idEvento, string idMedioDePago, MontoHistorialPagosVO montoPago, string ultimosDigitosTarjeta, string tipoMedioDePago)
        {
            Id = Guid.NewGuid();
            IdUsuario = idUsuario;
            IdEvento = idEvento;
            IdMedioDePago = idMedioDePago;
            MontoPago = montoPago;
            CreatedAt = DateTime.UtcNow;
            UltimosDigitosTarjeta = ultimosDigitosTarjeta;
            TipoMedioDePago = tipoMedioDePago;
        }


    }
}
