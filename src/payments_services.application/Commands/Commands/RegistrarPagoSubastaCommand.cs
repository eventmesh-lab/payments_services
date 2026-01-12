using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.DTOs;

namespace payments_services.application.Commands.Commands
{
    public class RegistrarPagoCommand : IRequest<bool>
    {
        /// <summary>
        /// Atributo DTO que se encarga de recibir la información del pago a registrar.
        /// </summary>
        public RegistrarPagoDTO medioDePagoDTO { get; set; }

        public RegistrarPagoCommand(RegistrarPagoDTO medioDePagoDTO)
        {

            this.medioDePagoDTO = medioDePagoDTO;
        }
    }
}
