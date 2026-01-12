using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.DTOs;

namespace payments_services.application.Queries.Queries
{
    public class ConsultarHistorialPagosUsuarioQuery : IRequest<List<HistorialPagosDTO>>
    {
        /// <summary>
        /// Atributo que contiene el correo del usuario de los pagoss a consultar.
        /// </summary>
        public string correo { get; set; }

        public ConsultarHistorialPagosUsuarioQuery(string correo)
        {

            this.correo = correo;
        }
    }
}
