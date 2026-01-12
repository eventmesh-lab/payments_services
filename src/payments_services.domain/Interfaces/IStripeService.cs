using payments_services.domain.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones que se pueden realizar sobre los medios de pagos y pagos, a través de la API de Stripe.
    /// </summary>
    public interface IStripeService
    {
        /// <summary>
        /// Metodo que se encarga de procesar la creación de un usuario en Stripe.
        /// </summary>
        /// <param name="usuarioId">Parametro que contiene el ID del usuario a registrar.</param>
        /// <returns>Retorna el ID del usuario en la base de datos de Stripe.</returns>
        Task<string> CrearUsuarioStripeAsync(string usuarioId);
        /// <summary>
        /// Metodo que se encarga de obtener el ID de un usuario en Stripe.
        /// </summary>
        /// <param name="usuarioId">Parametro que contiene el ID del usuario a consultar.</param>
        /// <returns>Retorna un valor string que contiene el ID del usuario en stripe.</returns>

        Task<string> ObtenerUsuarioStripeAsync(Guid usuarioId);
        /// <summary>
        /// Metodo que se encarga de registrar el pago de una reserva de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeClienteId">Parametro que contiene el ID del usuario a registrar el pago.</param>
        /// <param name="stripeMedioPagoId">Parametro que contiene el ID del medio de pago con el que se realiza el pago.</param>
        /// <param name="idReserva">Parametro que contiene el ID de la resreva a pagar.</param>
        /// <param name="idUsuario">Parametro que contiene el ID del usuario en la base de datos del Microservicio Usuarios.</param>
        /// <param name="moneda">Parametro que contiene la moneda con la que se realiza el pago.</param>
        /// <param name="monto">Parametro que contiene el monto con el que se realiza el pago.</param>
        /// <returns>Retorna un objeto PaymentIntent de la API de Stripe con los datos del pago.</returns>
        Task<PaymentIntent> RegistrarPagoAsync(string stripeClienteId, string stripeMedioPagoId, Guid idReserva, Guid idUsuario, string moneda, long monto);
        /// <summary>
        /// Metodo que se encarga de agregar un medio de pago a un usuario en Stripe.
        /// </summary>
        /// <param name="customerId">Parametro que contiene el ID del usuario a agregar el medio de pago.</param>
        /// <param name="paymentMethodId">Parametro que contiene el ID del medio de pago a agregar.</param>
        /// <returns>Retorna un estado HTTP exitoso si la respuesta de la API de stripe fue procesada.</returns>
        Task<HttpStatusCode> AgregarMedioDePagoStripeAsync(string customerId, string paymentMethodId);
        /// <summary>
        /// Metodo que se encarga de consultar los medios de pago de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeClienteId">Parametro que contiene el ID del usuario a consultar los medio de pago.</param>
        /// <returns>Retorna una lista de Medios de Pago con el detalle de cada uno.</returns>
        Task<List<MedioDePago>> ObtenerMediosDePagoStripeAsync(string stripeClienteId);
        /// <summary>
        /// Metodo que se encarga de establecer un medio de pago como predeterminado de un usuario en Stripe.
        /// </summary>
        /// <param name="clienteId">Parametro que contiene el ID del usuario a establecer el medio de pago.</param>
        /// <param name="medioPagoId">Parametro que contiene el ID del medio de pago a establecer.</param>
        /// <returns>Retorna un estado HTTP exitoso si la respuesta de la API de stripe fue procesada..</returns>

        Task<HttpStatusCode> EstablecerMedioDePagoPredeterminadoStripeAsync(string clienteId, string medioPagoId);
        /// <summary>
        /// Metodo que se encarga de consultar un medio de pago específico de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeClienteId">Parametro que contiene el ID del usuario a consultar el medio de pago.</param>
        /// <param name="stripeMedioPagoId">Parametro que contiene el ID del medio de pago a consultar.</param>
        /// <returns>Retorna un objeto Medios de Pago con el detalle del medio.</returns>
        Task<MedioDePago> ObtenerMedioDePagoStripeAsync(string stripeClienteId, string stripeMedioPagoId);
        /// <summary>
        /// Metodo que se encarga de eliminar un medio de pago de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeMedioDePagoId">Parametro que contiene el ID del medio de pago a eliminar.</param>
        /// <returns>Retorna un estado HTTP exitoso si la respuesta de la API de stripe fue procesada..</returns>

        Task<HttpStatusCode> EliminarMedioDePagoStripeAsync(string stripeMedioDePagoId);
        /// <summary>
        /// Metodo que se encarga de verificar si existe un usuario con el correo dado en Stripe.
        /// </summary>
        /// <param name="email">Parametro que contiene el correo del usuario a consultar.</param>
        /// <returns>Retorna un valor booleano dependiendo de la respuesta de la API de stripe.</returns>
        Task<bool> ExisteClienteConEmailAsync(string email);
    }
}
