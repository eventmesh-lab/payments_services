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
    /// Clase Adapter que se encarga de adaptar la interfaz de Stripe (CustomerService) a la interfaz IStripeCustomerService,
    /// encargada de gestionar las operaciones relacionadas con clientes en Stripe.
    /// </summary>
    public class StripeCustomerService : IStripeCustomerService
    {
        private readonly CustomerService _service;

        public StripeCustomerService(StripeClient client)
        {
            _service = new CustomerService(client);
        }

        /// <summary>
        /// Crea un nuevo cliente en Stripe con las opciones especificadas.
        /// </summary>
        /// <param name="options">Opciones utilizadas para crear el cliente.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cliente creado.</returns>
        public Task<Customer> CreateAsync(CustomerCreateOptions options) => _service.CreateAsync(options);

        /// <summary>
        /// Obtiene una lista de clientes desde Stripe según las opciones de filtrado proporcionadas.
        /// </summary>
        /// <param name="options">Opciones para filtrar y paginar la lista de clientes.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de objetos clientes/>.</returns>
        public Task<StripeList<Customer>> ListAsync(CustomerListOptions options) => _service.ListAsync(options);

        /// <summary>
        /// Recupera un cliente específico desde Stripe mediante su ID.
        /// </summary>
        /// <param name="customerId">ID del cliente a recuperar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cliente recuperado.</returns>
        public Task<Customer> GetAsync(string customerId) => _service.GetAsync(customerId);

        /// <summary>
        /// Actualiza un cliente existente en Stripe con las opciones especificadas.
        /// </summary>
        /// <param name="customerId">ID del cliente a actualizar.</param>
        /// <param name="options">Opciones que contienen la información actualizada del cliente.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cliente actualizado.</returns>
        public Task<Customer> UpdateAsync(string customerId, CustomerUpdateOptions options) => _service.UpdateAsync(customerId, options);
    }

}
