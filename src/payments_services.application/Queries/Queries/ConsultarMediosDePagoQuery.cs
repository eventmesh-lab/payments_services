using MediatR;
using payments_services.application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Queries.Queries
{
    /// <summary>
    /// Clase Query que se encarga de enviar la solicitud para consultar los medios de pago perteneciente a un usuario .
    /// </summary>
    public class ConsultarMediosDePagoQuery : IRequest<List<MedioDePagoDTO>>
    {
        /// <summary>
        /// Atributo que contiene el correo del usuario de los medio de pago a consultar.
        /// </summary>
        public string correo { get; set; }

        public ConsultarMediosDePagoQuery(string correo)
        {

            this.correo = correo;
        }
    }
}
