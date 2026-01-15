using MediatR;
using payments_services.application.DTOs;
using payments_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.Interfaces;
using payments_services.application.Queries.Queries;

namespace payments_services.application.Queries.Handlers
{
    public class ConsultarHistorialPagosUsuarioHandler : IRequestHandler<ConsultarHistorialPagosUsuarioQuery, List<HistorialPagosDTO>>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un historial de pagos, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IHistorialPagosRepositoryPostgres _historialPagosService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre una reserva en el Microservicio reserva, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IReservaService _reservaService;

        public ConsultarHistorialPagosUsuarioHandler(IUsuarioService usuarioService, IHistorialPagosRepositoryPostgres historialPagosService, IReservaService reservaService)
        {
            _historialPagosService = historialPagosService;
            _usuarioService = usuarioService;
            _reservaService = reservaService;
        }


        /// <summary>
        /// Metodo que se encarga de procesar la consulta de los pagos recibidos por un cliente.
        /// </summary>
        /// <param name="request">Parametro que contiene el el correo del cliente quien recibe los pagos.</param>
        /// <returns>Retorna un lista de DTOs con los datos de los pagos.</returns>
        public async Task<List<HistorialPagosDTO>> Handle(ConsultarHistorialPagosUsuarioQuery request, CancellationToken cancellationToken)
        {

            try
            {
                // Se obtiene el ID del usuario al que le pertenecen los pagos.
                var idUsuario = await _usuarioService.ObtenerUsuarioPorEmailAsync(request.correo);

                //En caso de que el ID del usuario retornado por la consulta sea vacío, se lanza la excepción
                if (idUsuario == Guid.Empty || idUsuario == null)
                    throw new ApplicationException("El usuario no existe en la base de datos.");

                //Se obtiene el historial de pagos del usuario en la base de datos 
                var historialPagos = await _historialPagosService.GetHistorialPagosByUserAsync(idUsuario, cancellationToken);

                //En caso de que la lista de pagos esté vacia, se lanza la excepción
                if (historialPagos == null || !historialPagos.Any())
                {
                    throw new ApplicationException("El usuario no posee pagos asociados.");
                }
                var listaHistorialPagos = new List<HistorialPagosDTO>();

                foreach (var pago in historialPagos)
                {
                   var usuario = await _usuarioService.ObtenerUsuarioPorEmail(request.correo);
                    listaHistorialPagos.Add(new HistorialPagosDTO
                    {
                        IdEvento = pago.IdEvento,
                        Id = pago.Id,
                        MontoPago = pago.MontoPago.montoPago,
                        CreatedAt = pago.CreatedAt,
                        UltimosDigitosTarjeta= pago.UltimosDigitosTarjeta,
                        idUsuario= pago.IdUsuario,
                        NombreUsuario =usuario.Nombre,
                        MetododePago = pago.TipoMedioDePago
                    });
                }


                return listaHistorialPagos;

            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Ha ocurrido un error al obtener el historial de pago en la bd", ex);
            }
        }
    }
}
