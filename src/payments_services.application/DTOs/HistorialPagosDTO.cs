using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    // <summary>
    /// Clase DTO que se encarga de mostrar la información de un pago de una reserva.
    /// </summary>
    public class HistorialPagosDTO
    {
      
        public Guid IdEvento { get; set; }
        
        public Guid Id { get; set; }
       
        public decimal MontoPago { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string UltimosDigitosTarjeta { get; set; }
        
        public Guid idUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string MetododePago { get; set; }
    }
}
