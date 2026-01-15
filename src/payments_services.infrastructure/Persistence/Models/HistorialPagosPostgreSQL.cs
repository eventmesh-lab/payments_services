using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.infrastructure.Persistence.Models
{
    public class HistorialPagosPostgreSQL
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }
        [Required]
        public Guid IdEvento { get; set; }
        [Required]
        public string IdMedioDePago { get; set; }
        [Required]
        public decimal Monto { get; set; }

        [Required]
        public string UltimosCuatroDigitos { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public string TipoMedioDePago { get; set; }

    }
}
