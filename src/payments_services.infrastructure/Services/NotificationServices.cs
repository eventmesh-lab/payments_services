using payments_services.application.DTOs;
using payments_services.domain.Interfaces;
using System.Net.Http.Json;

namespace payments_services.infrastructure.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly HttpClient _httpClient;
        public NotificationServices (HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task EnviarPagoExitoso(string userId, string amount)
        {
            try
            {
                var dto = new PaymentSuccessNotificationDto(
                    userId,
                    amount
                );

                var response = await _httpClient.PostAsJsonAsync($"http://localhost:7184/api/notification/paymentSuccessNotification", dto);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Error al enviar la notificacion");
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task ReservaConfirmadaExitos(string email)
        {
            try
            {
                var dto = new ConfirmedReservationDto(
                    email
                );

                var response = await _httpClient.PostAsJsonAsync($"http://localhost:7184/api/notification/ConfirmedReservationNotification", dto);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Error al enviar la notificacion");
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
