using payments_services.application.DTOs;
using payments_services.application.Interfaces;
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

        public async Task EnviarCorreoPagoExitosoDetallado(EnviarCorreoPagoExitosoDto dto)
        {
            try
            {
                // Asumiendo que el controlador está en api/notification según tus otros endpoints
                var url = "http://localhost:7184/api/notification/paymentSuccessNotificationEmail";

                var response = await _httpClient.PostAsJsonAsync(url, dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    throw new ArgumentException($"Error al enviar el correo de pago detallado. Status: {response.StatusCode}, Detalle: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                // Aquí podrías agregar un Logger
                Console.WriteLine($"Error en NotificationServices: {ex.Message}");
                throw;
            }
        }
    }
}
