using MediatR;
using payments_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.Commands.Commands;

namespace payments_services.application.Commands.Handlers
{
    /// <summary>
    /// Clase Handler que se encarga registrar un medio de pagos de un usuario en la base de datos de Stripe.
    /// </summary>
    public class RegistrarMedioDePagoHandler : IRequestHandler<RegistrarMedioPagoCommand, bool>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un pago o medio de pago con Stripe, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IStripeService _stripeService;
        private readonly INotificationServices _notificationServices;

        public RegistrarMedioDePagoHandler(IUsuarioService usuarioService, IStripeService stripeService, INotificationServices notificationServices)
        {
            _stripeService = stripeService;
            _usuarioService = usuarioService;
            _notificationServices = notificationServices;
        }

        /// <summary>
        /// Metodo que se encarga de procesar la solicitud de registrar  un medio de pago de un usuario.
        /// </summary>
        /// <param name="request">Parametro que contiene el correo del usuario perteneciente del medio de pago, y un token con el ID del medio de pago a registrar.</param>
        /// <returns>Retorna un valor booleano si el medio de pago fue registrado como exitosamente.</returns>
        public async Task<bool> Handle(RegistrarMedioPagoCommand request, CancellationToken cancellationToken)
        {

            try
            {
                // Se obtiene el ID del usuario al que le pertenece el medio de pago.
                var idUsuario = await _usuarioService.ObtenerUsuarioPorEmailAsync(request.medioPagoDTO.correo);
                
                //En caso de que el ID del usuario retornado por la consulta sea vacío, se lanza la excepción
                if (idUsuario == Guid.Empty || idUsuario == null)
                    throw new ApplicationException($"El usuario no existe en la base de datos.");
                
                // Se verifica si el usuario existe en la base de datos de Stripe
                var existeUsuario = await _stripeService.ExisteClienteConEmailAsync(request.medioPagoDTO.correo);
                
                string idUsuarioStripe;
                
                if (!existeUsuario)
                {
                    // Se crea el usuario en la base de datos de Stripe si no existe.
                    idUsuarioStripe = await _stripeService.CrearUsuarioStripeAsync(request.medioPagoDTO.correo);
                    
                }
                else
                {

                    // Se obtiene el ID del usuario en la base de datos de Stripe si existe.
                    idUsuarioStripe = await _stripeService.ObtenerUsuarioStripeAsync(idUsuario);
                   
                }

                // Se le añade el medio de pago al usuario en la base de datos de Stripe.
                var medioDePago = await _stripeService.AgregarMedioDePagoStripeAsync(idUsuarioStripe, request.medioPagoDTO.medioPagoStripeID);
               
                //En caso de que el medio de pago no pudo ser registrado, se lanza la excepción
                if (medioDePago != System.Net.HttpStatusCode.OK)
                    throw new ApplicationException($"Fallo al registrar medio de pago.");
            //    await _notificationServices.ReservaConfirmadaExitos(request.medioPagoDTO.correo);
                return true;

            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Ha ocurrido un error al registrar el medio de pago en la stripe", ex);
            }
        }
    }
}
