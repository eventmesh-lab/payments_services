using MediatR;
using payments_services.application.DTOs;
using payments_services.application.Queries.Queries;
using payments_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Queries.Handlers
{
    /// <summary>
    /// Clase Handler que se encarga consultar un medio de pago perteneciente a un usuario en la bases de datos en Stripe.
    /// </summary>
    public class ConsultarMedioDePagoHandler : IRequestHandler<ConsultarMedioDePagoQuery, MedioDePagoDTO>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un pago o medio de pago con Stripe, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IStripeService _stripeService;

        public ConsultarMedioDePagoHandler(IUsuarioService usuarioService, IStripeService stripeService)
        {
            _stripeService = stripeService;
            _usuarioService = usuarioService;
        }
        /// <summary>
        /// Metodo que se encarga de procesar la consulta de un medio de pago.
        /// </summary>
        /// <param name="request">Parametro que contiene el ID del medio de pago y el correo del usuario dueño del medio de pago.</param>
        /// <returns>Retorna un DTO con los datos del medio de pago.</returns>
        public async Task<MedioDePagoDTO> Handle(ConsultarMedioDePagoQuery request, CancellationToken cancellationToken)
        {

            try
            {
                // Se obtiene el ID del usuario al que le pertenece el medio de pago dado.
                var idUsuario = await _usuarioService.ObtenerUsuarioPorEmailAsync(request.medioDePagoDTO.correo);

                //En caso de que el Id del usuario retornado por la consulta sea vacío, se lanza la excepción
                if (idUsuario == Guid.Empty || idUsuario == null)
                    throw new ApplicationException($"El usuario no existe en la base de datos.");

                //Se obtiene el ID del usuario en la base de datos de Stripe
                var idUsuarioStripe = await _stripeService.ObtenerUsuarioStripeAsync(idUsuario);

                //Se obtiene el medio de pago del usuario en la base de datos de Stripe
                var medioPago = await _stripeService.ObtenerMedioDePagoStripeAsync(idUsuarioStripe, request.medioDePagoDTO.idMedioDePagoStripe);

                //En caso de que el medio de pago esté vacio, se lanza la excepción
                if (medioPago == null)
                    throw new ApplicationException($"No existen este medio de pago asociado al usuario.");

                var medioDePagoDTO = new MedioDePagoDTO
                {
                    idMedioPago = medioPago.StripeMedioPagoId,
                    medioPredeterminado = medioPago.MedioPredeterminado.medioPredeterminado,
                    tipoMedioPago = medioPago.TipoTarjeta.tipoPago,
                    ultimosCuatroDigitos = medioPago.UltimosDigitosTarjeta

                };

                return medioDePagoDTO;

            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Ha ocurrido un error al obtener los medio de pago en la stripe", ex);
            }
        }
    }
}
