using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using payments_services.domain.Entities;
using payments_services.domain.Interfaces;
using payments_services.domain.Factory;
using payments_services.application.Interfaces;

namespace payments_services.infrastructure.ExternalServices.Stripe.Services
{
    /// <summary>
    /// Clase external service que se encarga de manejar las operaciones con los pagos y medios de pago posibles a través de la API de Stripe.
    /// </summary>
    public class StripeService : IStripeService
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones sobre el cliente de Stripe.
        /// </summary>
        private readonly IStripeCustomerService _customerService;
        /// <summary>
        /// Atributo que corresponde a las operaciones sobre los métodos de pago de un cliente de Stripe.
        /// </summary>
        private readonly IStripePaymentMethodService _paymentMethodService;
        /// <summary>
        /// Atributo que corresponde a las operaciones sobre pagos de un cliente de Stripe.
        /// </summary>
        private readonly IStripePaymentIntentService _paymentIntentService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;

        public StripeService(IStripeCustomerService customerService, IStripePaymentMethodService paymentMethodService, IUsuarioService usuarioService, IStripePaymentIntentService paymentIntentService)
        {
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
            _usuarioService = usuarioService;
            _paymentIntentService = paymentIntentService;
        }


        /// <summary>
        /// Metodo que se encarga de procesar la creación de un usuario en Stripe.
        /// </summary>
        /// <param name="usuarioId">Parametro que contiene el ID del usuario a registrar.</param>
        /// <returns>Retorna el ID del usuario en la base de datos de Stripe.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<string> CrearUsuarioStripeAsync(string correo)
        {
            var usuarioId = await _usuarioService.ObtenerUsuarioPorEmailAsync(correo);

            var opciones = new CustomerCreateOptions
            {
                Email = correo,
                Metadata = new Dictionary<string, string>
                  {
                      { "usuarioId", usuarioId.ToString() }
                  }
            };

            var cliente = await _customerService.CreateAsync(opciones);
            return cliente.Id;
        }

        /// <summary>
        /// Metodo que se encarga de verificar si existe un usuario con el correo dado en Stripe.
        /// </summary>
        /// <param name="email">Parametro que contiene el correo del usuario a consultar.</param>
        /// <returns>Retorna un valor booleano dependiendo de la respuesta de la API de stripe.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<bool> ExisteClienteConEmailAsync(string email)
        {

            var opciones = new CustomerListOptions { Limit = 100 };
            var lista = await _customerService.ListAsync(opciones);

            return lista.Data.Any(c =>
                    !string.IsNullOrWhiteSpace(c.Email) &&
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        }

        /// <summary>
        /// Metodo que se encarga de obtener el ID de un usuario en Stripe.
        /// </summary>
        /// <param name="usuarioId">Parametro que contiene el ID del usuario a consultar.</param>
        /// <returns>Retorna un valor string que contiene el ID del usuario en stripe.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<string> ObtenerUsuarioStripeAsync(Guid usuarioId)
        {
            var opciones = new CustomerListOptions { Limit = 100 };
            var customers = await _customerService.ListAsync(opciones);

            var cliente = customers.Data.FirstOrDefault(c =>
                c.Metadata != null &&
                c.Metadata.TryGetValue("usuarioId", out var id) &&
                id == usuarioId.ToString());

            return cliente?.Id;
        }

        /// <summary>
        /// Metodo que se encarga de agregar un medio de pago a un usuario en Stripe.
        /// </summary>
        /// <param name="customerId">Parametro que contiene el ID del usuario a agregar el medio de pago.</param>
        /// <param name="paymentMethodId">Parametro que contiene el ID del medio de pago a agregar.</param>
        /// <returns>Retorna un estado HTTP exitoso si la respuesta de la API de stripe fue procesada.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<HttpStatusCode> AgregarMedioDePagoStripeAsync(string customerId, string paymentMethodId)
        {
            await _paymentMethodService.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions
            {
                Customer = customerId
            });

            return HttpStatusCode.OK;

        }

        /// <summary>
        /// Metodo que se encarga de consultar los medios de pago de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeClienteId">Parametro que contiene el ID del usuario a consultar los medio de pago.</param>
        /// <returns>Retorna una lista de Medios de Pago con el detalle de cada uno.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<List<MedioDePago>> ObtenerMediosDePagoStripeAsync(string stripeClienteId)
        {
            try
            {
                var medios = new List<MedioDePago>();
                Console.WriteLine("hola1");
                var cliente = await _customerService.GetAsync(stripeClienteId);
                
                var idPredeterminado = cliente.InvoiceSettings.DefaultPaymentMethodId;
                Console.WriteLine("hola3");
                var lista = await _paymentMethodService.ListAsync(new PaymentMethodListOptions
                {
                    Customer = stripeClienteId,
                    Type = "card"
                });
                Console.WriteLine("hola4");
                foreach (var stripeMedio in lista.Data)
                {
                    var medio = MedioDePagoFactory.CrearMedioDePago(
                        Guid.Parse(cliente.Metadata["usuarioId"]),
                        stripeClienteId,
                        stripeMedio.Id,
                        stripeMedio.Card.Last4,
                        stripeMedio.Card.Brand,
                        stripeMedio.Id == idPredeterminado
                    );
                    Console.WriteLine("hola");
                    medios.Add(medio);
                }

                Console.WriteLine("hola5");
                return medios;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Error al obtener los datos del metodo de pago", ex);
            }
        }

        /// <summary>
        /// Metodo que se encarga de consultar un medio de pago específico de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeClienteId">Parametro que contiene el ID del usuario a consultar el medio de pago.</param>
        /// <param name="stripeMedioPagoId">Parametro que contiene el ID del medio de pago a consultar.</param>
        /// <returns>Retorna un objeto Medios de Pago con el detalle del medio.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<MedioDePago> ObtenerMedioDePagoStripeAsync(string stripeClienteId, string stripeMedioPagoId)
        {
            var cliente = await _customerService.GetAsync(stripeClienteId);
            if (!cliente.Metadata.TryGetValue("usuarioId", out var usuarioId))
                return null;

            var idPredeterminado = cliente.InvoiceSettings.DefaultPaymentMethodId;

            var lista = await _paymentMethodService.ListAsync(new PaymentMethodListOptions
            {
                Customer = stripeClienteId,
                Type = "card"
            });

            var stripeMedio = lista.Data.FirstOrDefault(pm => pm.Id == stripeMedioPagoId);
            if (stripeMedio == null)
                return null;

            var esPredeterminado = idPredeterminado == stripeMedio.Id;

            return MedioDePagoFactory.CrearMedioDePago(
                Guid.Parse(usuarioId),
                stripeMedio.CustomerId,
                stripeMedio.Id,
                stripeMedio.Card.Last4,
                stripeMedio.Card.Brand,
                esPredeterminado
            );

        }
        /// <summary>
        /// Metodo que se encarga de establecer un medio de pago como predeterminado de un usuario en Stripe.
        /// </summary>
        /// <param name="clienteId">Parametro que contiene el ID del usuario a establecer el medio de pago.</param>
        /// <param name="medioPagoId">Parametro que contiene el ID del medio de pago a establecer.</param>
        /// <returns>Retorna un estado HTTP exitoso si la respuesta de la API de stripe fue procesada..</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<HttpStatusCode> EstablecerMedioDePagoPredeterminadoStripeAsync(string clienteId, string medioPagoId)
        {
            try
            {
                await _customerService.UpdateAsync(clienteId, new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = medioPagoId
                    }
                });

                return HttpStatusCode.OK;
            }
            catch (StripeException)
            {
                return HttpStatusCode.InternalServerError;
            }

        }

        /// <summary>
        /// Metodo que se encarga de eliminar un medio de pago de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeMedioDePagoId">Parametro que contiene el ID del medio de pago a eliminar.</param>
        /// <returns>Retorna un estado HTTP exitoso si la respuesta de la API de stripe fue procesada..</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<HttpStatusCode> EliminarMedioDePagoStripeAsync(string stripeMedioDePagoId)
        {
            try
            {
                await _paymentMethodService.DetachAsync(stripeMedioDePagoId);
                return HttpStatusCode.OK;
            }
            catch (StripeException)
            {
                return HttpStatusCode.InternalServerError;
            }

        }
        /// <summary>
        /// Metodo que se encarga de registrar el pago de una reserva de un usuario en Stripe.
        /// </summary>
        /// <param name="stripeClienteId">Parametro que contiene el ID del usuario a registrar el pago.</param>
        /// <param name="stripeMedioPagoId">Parametro que contiene el ID del medio de pago con el que se realiza el pago.</param>
        /// <param name="idReserva">Parametro que contiene el ID de la reserva a pagar.</param>
        /// <param name="idUsuario">Parametro que contiene el ID del usuario en la base de datos del Microservicio Usuarios.</param>
        /// <param name="moneda">Parametro que contiene la moneda con la que se realiza el pago.</param>
        /// <param name="monto">Parametro que contiene el monto con el que se realiza el pago.</param>
        /// <returns>Retorna un objeto PaymentIntent de la API de Stripe con los datos del pago.</returns>
        /// <exception cref="StripeException">
        /// Esta excepcion si sucede un error inesperado con Stripe.
        /// </exception>
        public async Task<PaymentIntent> RegistrarPagoAsync(string stripeClienteId, string stripeMedioPagoId, Guid idReserva, Guid idUsuario, string moneda, long monto)
        {
            var cliente = await _customerService.GetAsync(stripeClienteId);

            var opciones = new PaymentIntentCreateOptions
            {
                Customer = stripeClienteId,
                Amount = monto,
                Currency = moneda,
                PaymentMethod = stripeMedioPagoId,
                Confirm = true,
                OffSession = true,
                Metadata = new Dictionary<string, string>
                {
                    { "reservaId", idReserva.ToString() },
                    { "usuarioId", idUsuario.ToString() }
                }
            };

            return await _paymentIntentService.CreateAsync(opciones);


        }
        public async Task<string> ObtenerIdClientePorEmailAsync(string email)
        {
            var opciones = new CustomerListOptions
            {
                Email = email,
                Limit = 1
            };
            var clientes = await _customerService.ListAsync(opciones);

            // Devuelve el ID del primero que encuentre, o null si no hay.
            return clientes.Data.FirstOrDefault()?.Id;
        }
    }
}
