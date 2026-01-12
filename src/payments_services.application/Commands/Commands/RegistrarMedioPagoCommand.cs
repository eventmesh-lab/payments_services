using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.DTOs;

namespace payments_services.application.Commands.Commands
{
    /// <summary>
    /// Clase Command que se encarga de enviar la solicitud de querer registar  un medio de pago.
    /// </summary>
    public class RegistrarMedioPagoCommand : IRequest<bool>
    {
        /// <summary>
        /// Atributo DTO que se encarga de recibir la información del medio de pago a registrar.
        /// </summary>
        public RegistrarMedioDePagoDTO medioPagoDTO;

        public RegistrarMedioPagoCommand(RegistrarMedioDePagoDTO mediopagodto)
        {
            medioPagoDTO = mediopagodto;

        }
    }
}
