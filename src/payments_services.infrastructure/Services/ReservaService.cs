using payments_services.application.DTOs;
using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using payments_services.application.Interfaces;

namespace payments_services.infrastructure.Services
{
    public class ReservaService : IReservaService
    {
        /// <summary>
        /// Atributo que se encarga de procesar las solicitudes a servicios externos.
        /// </summary>
        private readonly HttpClient _httpClient;

        public ReservaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Reserva> ObtenerReservaPorGuid(Guid idReserva)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5003/api/Reservas/obtenerReserva/{idReserva}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var contenido = await response.Content.ReadAsStringAsync();
                Console.WriteLine(contenido);

                var dto = JsonSerializer.Deserialize<ReservaDto>(contenido, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dto == null)
                {
                    return null;
                }

                var reserva = new Reserva(
                    dto.Id,
                    dto.idUsuario,
                    dto.montoTotal,
                    dto.IdEvento
                );

                return reserva;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
    }
}
