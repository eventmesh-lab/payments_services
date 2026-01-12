using MediatR;
using payments_services.application.DTOs;
using payments_services.application.Interfaces;
using payments_services.application.Queries.Queries;
using payments_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Queries.Handlers
{
    public class ConsultarPagosHandler : IRequestHandler<ConsultarPagosQuery, List<HistorialPagosDTO>>
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

        public ConsultarPagosHandler(IUsuarioService usuarioService, IHistorialPagosRepositoryPostgres historialPagosService, IReservaService reservaService)
        {
            _historialPagosService = historialPagosService;
            _usuarioService = usuarioService;
            _reservaService = reservaService;
        }


        /// <summary>
        /// Metodo que se encarga de procesar la consulta de los pagos recibidos por un usuario.
        /// </summary>
        /// <param name="request">Parametro que contiene el correo del usuario quien recibe los pagos.</param>
        /// <returns>Retorna un lista de DTOs con los datos de los pagos.</returns>
        public async Task<List<HistorialPagosDTO>> Handle(ConsultarPagosQuery request, CancellationToken cancellationToken)
        {

            try
            {

                //Se obtiene el historial de pagos del usuario en la base de datos 
                var historialPagos = await _historialPagosService.GetAllPagosAsync(cancellationToken);

                //En caso de que la lista de pagos esté vacia, se lanza la excepción
                if (historialPagos == null || !historialPagos.Any())
                {
                    throw new ApplicationException("No hay pagos registrados.");
                }
                var listaHistorialPagos = new List<HistorialPagosDTO>();

                foreach (var pago in historialPagos)
                {
                    //Se obtiene la reserva correspondiente al pago desde el Microservicio reservas
                    var reserva = await _reservaService.ObtenerReservaPorGuid(pago.IdReserva);
                    var usuario = await _usuarioService.ObtenerUsuarioPorId(pago.IdUsuario);
                    listaHistorialPagos.Add(new HistorialPagosDTO
                    {
                        IdReserva = pago.IdReserva,
                        IdEvento = reserva.IdEvento,
                        Id = pago.Id,
                        MontoPago = pago.MontoPago.montoPago,
                        CreatedAt = pago.CreatedAt,
                        UltimosDigitosTarjeta = pago.UltimosDigitosTarjeta,
                        idUsuario = pago.IdUsuario,
                        NombreUsuario = usuario.Nombre,
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
