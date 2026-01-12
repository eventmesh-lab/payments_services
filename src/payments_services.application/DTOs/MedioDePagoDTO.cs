using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.DTOs
{
    /// <summary>
    /// Clase DTO que se encarga de encapsular la información necesaria para mostrar un medio de pago registrado previamente.
    /// </summary>
    public class MedioDePagoDTO
    {
        /// <summary>
        /// Atributo que corresponde al ID del medio de pago .
        /// </summary>
        public string idMedioPago { get; set; }
        /// <summary>
        /// Atributo que corresponde al tipo de medio de pago .
        /// </summary>
        public string tipoMedioPago { get; set; }
        /// <summary>
        /// Atributo que corresponde a determinar si el medio de pago es predeterminado (True, False) .
        /// </summary>
        public bool medioPredeterminado { get; set; }
        /// <summary>
        /// Atributo que corresponde a los ultimos cuatro digitos del medio de pago .
        /// </summary>
        public string ultimosCuatroDigitos { get; set; }
    }
}
