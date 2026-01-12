using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Interfaces
{
    /// <summary>
    /// Clase interface que se encarga de definir las operaciones relacionadas con los métodos de pago en Stripe.
    /// </summary>
    public interface IStripePaymentMethodService
    {
        /// <summary>
        /// Asocia un método de pago existente a un cliente en Stripe.
        /// </summary>
        /// <param name="paymentMethodId">ID del método de pago a asociar.</param>
        /// <param name="options">Opciones que especifican el cliente y configuración adicional.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el medio de pago asociado.</returns>
        Task<PaymentMethod> AttachAsync(string paymentMethodId, PaymentMethodAttachOptions options);
        /// <summary>
        /// Obtiene una lista de métodos de pago según los criterios especificados.
        /// </summary>
        /// <param name="options">Opciones para filtrar los métodos de pago.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de medios de pago.</returns>
        Task<StripeList<PaymentMethod>> ListAsync(PaymentMethodListOptions options);
        /// <summary>
        /// Desasocia un método de pago previamente vinculado a un cliente.
        /// </summary>
        /// <param name="paymentMethodId">ID del método de pago a desasociar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el medio de pago desasociado.</returns>
        Task<PaymentMethod> DetachAsync(string paymentMethodId);
    }
}
