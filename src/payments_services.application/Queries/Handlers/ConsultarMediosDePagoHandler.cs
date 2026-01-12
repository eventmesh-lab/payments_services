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
    /// Clase Handler que se encarga consultar los medios de pago perteneciente a un usuario en la bases de datos en Stripe.
    /// </summary>
    public class ConsultarMediosDePagoHandler : IRequestHandler<ConsultarMediosDePagoQuery, List<MedioDePagoDTO>>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un pago o medio de pago con Stripe, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IStripeService _stripeService;

        public ConsultarMediosDePagoHandler(IUsuarioService usuarioService, IStripeService stripeService)
        {
            _stripeService = stripeService;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Metodo que se encarga de procesar la consulta de los medios de pago de un usuario.
        /// </summary>
        /// <param name="request">Parametro que contiene el el correo del usuario dueño de los medios de pago.</param>
        /// <returns>Retorna un lista de DTOs con los datos del medio de pago.</returns>
        
        public async Task<List<MedioDePagoDTO>> Handle(ConsultarMediosDePagoQuery request, CancellationToken cancellationToken)
        {

            try
            {
                // Se obtiene el ID del usuario al que le pertenecen los medio de pago dado.
                var idUsuario = await _usuarioService.ObtenerUsuarioPorEmailAsync(request.correo);
                Console.WriteLine($"hola {idUsuario}");
                if (idUsuario == Guid.Empty)
                {
                    throw new ArgumentException("El usuario no existe en la base de datos");
                }
                //En caso de que el ID del usuario retornado por la consulta sea vacío, se lanza la excepción
                if (idUsuario == Guid.Empty || idUsuario == null)
                    throw new ApplicationException($"El usuario no existe en la base de datos.");
                Console.WriteLine("hola");
                //Se obtiene el ID del usuario en la base de datos de Stripe
                var idUsuarioStripe = await _stripeService.ObtenerUsuarioStripeAsync(idUsuario);
                Console.WriteLine("hola");
                //Se obtienen los medios de pago del usuario en la base de datos de Stripe
                var mediosDePago = await _stripeService.ObtenerMediosDePagoStripeAsync(idUsuarioStripe);
                Console.WriteLine("hola");
                //En caso de que la lista de medio de pago esté vacia, se lanza la excepción
                if (mediosDePago == null || !mediosDePago.Any())
                {
                    throw new ApplicationException($"No existen medios de pago asociados al usuario.");
                }
                var listaMediosDePago = new List<MedioDePagoDTO>();
                Console.WriteLine("hola");
                foreach (var medioPago in mediosDePago)
                {

                    listaMediosDePago.Add(new MedioDePagoDTO
                    {
                        idMedioPago = medioPago.StripeMedioPagoId,
                        medioPredeterminado = medioPago.MedioPredeterminado.medioPredeterminado,
                        tipoMedioPago = medioPago.TipoTarjeta.tipoPago,
                        ultimosCuatroDigitos = medioPago.UltimosDigitosTarjeta

                    });
                }


                return listaMediosDePago;

            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Ha ocurrido un error al obtener los medio de pago en stripe", ex);
            }
        }
    }
}
