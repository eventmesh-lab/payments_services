using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace payments_services.domain.Entities
{
    public class Reserva
    {
        public Guid Id { get; set; }
        public decimal MontoTotal { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid IdEvento { get; set; }

        public Reserva( Guid idUsuario, decimal montoTotal, Guid evento)
        {
            Id = Guid.NewGuid();
            MontoTotal = montoTotal;
            IdUsuario = idUsuario;
            IdEvento = evento;
        }

        [JsonConstructor]
        public Reserva(Guid id, Guid idUsuario, decimal montoTotal, Guid evento)
        {
            Id = id;
            MontoTotal = montoTotal;
            IdUsuario = evento;
            IdEvento = evento;
        }

    }
   
}
