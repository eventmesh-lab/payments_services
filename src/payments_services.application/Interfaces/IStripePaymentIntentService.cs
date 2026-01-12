using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Interfaces
{
    /// <summary>
    /// Clase interface que se encarga de definir las operaciones relacionadas con los pagos en Stripe.
    /// </summary>
    public interface IStripePaymentIntentService
    {
        /// <summary>
        /// Crea un nuevo intento de pago en Stripe con las opciones especificadas.
        /// </summary>
        /// <param name="options">Opciones utilizadas para configurar el intent de pago.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el intento de pago creado.</returns>
        Task<PaymentIntent> CreateAsync(PaymentIntentCreateOptions options);
    }
}
