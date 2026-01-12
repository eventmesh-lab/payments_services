using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Interfaces
{
    public interface IStripeCustomerService
    {
        /// <summary>
        /// Crea un nuevo cliente en Stripe con las opciones especificadas.
        /// </summary>
        /// <param name="options">Opciones utilizadas para crear el cliente.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cliente creado.</returns>
        Task<Customer> CreateAsync(CustomerCreateOptions options);
        /// <summary>
        /// Obtiene una lista de clientes desde Stripe según las opciones de filtrado proporcionadas.
        /// </summary>
        /// <param name="options">Opciones para filtrar y paginar la lista de clientes.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene una lista de objetos clientes/>.</returns>
        Task<StripeList<Customer>> ListAsync(CustomerListOptions options);
        /// <summary>
        /// Recupera un cliente específico desde Stripe mediante su ID.
        /// </summary>
        /// <param name="customerId">ID del cliente a recuperar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cliente recuperado.</returns>
        Task<Customer> GetAsync(string customerId);
        /// <summary>
        /// Actualiza un cliente existente en Stripe con las opciones especificadas.
        /// </summary>
        /// <param name="customerId">ID del cliente a actualizar.</param>
        /// <param name="options">Opciones que contienen la información actualizada del cliente.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el cliente actualizado.</returns>
        Task<Customer> UpdateAsync(string customerId, CustomerUpdateOptions options);


    }

}
