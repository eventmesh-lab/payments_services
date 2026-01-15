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
    /// Clase Query que se encarga de enviar la solicitud para consultar el un medio de pago perteneciente a un usuario .
    /// </summary>
    public class ConsultarMedioDePagoQuery : IRequest<MedioDePagoDTO>
    {
        /// <summary>
        /// Atributo que contiene el ID del medio de pago a consultar.
        /// </summary>
        public ConsultarMediosDePagoDTO medioDePagoDTO { get; set; }

        public ConsultarMedioDePagoQuery(ConsultarMediosDePagoDTO medioDePagoDTO)
        {

            this.medioDePagoDTO = medioDePagoDTO;
        }
    }
}
