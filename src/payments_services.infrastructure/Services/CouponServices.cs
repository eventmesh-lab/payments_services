using payments_services.application.DTOs;
using payments_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.infrastructure.Services
{
    public class CouponServices : ICouponServices
    {
        private readonly HttpClient _httpClient;
        public CouponServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task MarcarCuponComoUsado(Guid id)
        {
            try
            {
                Console.WriteLine("cupon");
                var response = await _httpClient.PutAsJsonAsync($"http://localhost:7185/api/coupons/updateUser/{id}", id);
                Console.WriteLine("cupon");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Error al actualizar el cupon");
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
