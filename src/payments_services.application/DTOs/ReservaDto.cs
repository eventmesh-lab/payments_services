using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    public class ReservaDto
    {
        /// <summary>
        /// Atributo que corresponde al ID de la reserva.
        /// </summary>
        [JsonPropertyName("idReserva")]
        public Guid Id { get; set; }
        public decimal montoTotal { get; set; }
        public Guid idUsuario { get; set; }
        public Guid IdEvento { get; set; }
    }
}

