using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.Interfaces;

namespace payments_services.infrastructure.ExternalServices.Stripe.Adapters
{
    /// <summary>
    /// Clase Adapter que se encarga de adaptar la interfaz de Stripe (PaymentIntentService) a la interfaz IStripePaymentIntentService,
    /// encargada de gestionar las operaciones relacionadas con los pagos en Stripe.
    /// </summary>
    public class StripePaymentIntentService : IStripePaymentIntentService
    {
        private readonly PaymentIntentService _service;

        public StripePaymentIntentService(StripeClient client)
        {
            _service = new PaymentIntentService(client);
        }

        /// <summary>
        /// Crea un nuevo intento de pago en Stripe con las opciones especificadas.
        /// </summary>
        /// <param name="options">Opciones utilizadas para configurar el intent de pago.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el intento de pago creado.</returns>
        public Task<PaymentIntent> CreateAsync(PaymentIntentCreateOptions options)
        {
            return _service.CreateAsync(options);
        }
    }

}
