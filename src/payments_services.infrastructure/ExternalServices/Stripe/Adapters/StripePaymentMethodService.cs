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
    /// Clase Adapter que se encarga de adaptar la interfaz de Stripe (PaymentMethodService) a la interfaz IStripePaymentMethodService,
    /// encargada de gestionar las operaciones relacionadas con los medios de pago en Stripe.
    /// </summary>
    public class StripePaymentMethodService : IStripePaymentMethodService
    {
        private readonly PaymentMethodService _service;

        public StripePaymentMethodService(StripeClient client)
        {
            _service = new PaymentMethodService(client);
        }

        /// <summary>
        /// Asocia un método de pago existente a un cliente en Stripe.
        /// </summary>
        /// <param name="paymentMethodId">ID del método de pago a asociar.</param>
        /// <param name="options">Opciones que especifican el cliente y configuración adicional.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el medio de pago asociado.</returns>
        public Task<PaymentMethod> AttachAsync(string paymentMethodId, PaymentMethodAttachOptions options)
        {
            return _service.AttachAsync(paymentMethodId, options);
        }

        /// <summary>
        /// Obtiene una lista de métodos de pago según los criterios especificados.
        /// </summary>
        /// <param name="options">Opciones para filtrar los métodos de pago.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de medios de pago.</returns>
        public Task<StripeList<PaymentMethod>> ListAsync(PaymentMethodListOptions options)
        {
            return _service.ListAsync(options);
        }

        /// <summary>
        /// Desasocia un método de pago previamente vinculado a un cliente.
        /// </summary>
        /// <param name="paymentMethodId">ID del método de pago a desasociar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el medio de pago desasociado.</returns>
        public Task<PaymentMethod> DetachAsync(string paymentMethodId)
        {
            return _service.DetachAsync(paymentMethodId);
        }
    }
}
