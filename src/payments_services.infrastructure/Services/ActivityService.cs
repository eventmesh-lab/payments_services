using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.DTOs;
using payments_services.application.Interfaces;

namespace payments_services.infrastructure.Services
{
    public class ActivityService : IActivityService
    {
        private readonly HttpClient _httpClient;

        public ActivityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterActivityAsync(string email, string action, string category)
        {
            try
            {
                var requestBody = new ActivityRequestDto
                {
                    Action = action,
                    Category = category
                };

                var response = await _httpClient.PostAsJsonAsync($"api/activityhistory/registerActivity/{email}", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al registrar actividad: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción conectando con ActivityService: {ex.Message}");
                return false;
            }
        }
    }
}
