using payments_services.application.DTOs;
using payments_services.application.Interfaces;
using payments_services.domain.Entities;
using payments_services.domain.Interfaces;
using Stripe.Forwarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace payments_services.infrastructure.Services
{
    /// <summary>
    /// Clase Service que se encarga de procesar todas las operaciones sobre un usuario, realizando peticiones HTTP al Microservicio Usuarios.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {

        /// <summary>
        /// Atributo que se encarga de procesar las solicitudes a servicios externos.
        /// </summary>
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// Método que se encarga de obtener el ID de un usuario por su correo en el Microservicio Usuarios.
        /// </summary>
        /// <param name="correo">Parametro que corresponde al correo del usuario a consultar</param>
        /// <returns>Retorna un valor GUID que corresponde al ID del usuario consultado.
        /// Si no lo consigue, retorna un GUID vacio</returns>
        public async Task<Guid> ObtenerUsuarioPorEmailAsync(string correo)
        {
            var response = await _httpClient.GetAsync($"http://localhost:7181/api/users/getIdUser/{correo}");

            if (!response.IsSuccessStatusCode)
            {
                return Guid.Empty;
            }

            var guidString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🔹 GUID recibido desde el microservicio (antes de conversión): {guidString}");

            if (Guid.TryParse(guidString.Trim('"'), out Guid userId))
            {
                return userId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public async Task<Guid> ObtenerUsuarioPorIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:7181/api/users/getIdUser/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return Guid.Empty;
            }

            var guidString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🔹 GUID recibido desde el microservicio (antes de conversión): {guidString}");

            if (Guid.TryParse(guidString.Trim('"'), out Guid userId))
            {
                return userId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public async Task<Usuario> ObtenerUsuarioPorEmail(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:7181/api/users/getUser/{email}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var contenido = await response.Content.ReadAsStringAsync();
                Console.WriteLine(contenido);

                var dto = JsonSerializer.Deserialize<UsuarioDto>(contenido, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dto == null)
                {
                    return null;
                }

                var usuario = new Usuario(
                    dto.Nombre,
                    dto.Email
                );

                return usuario;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public async Task<Usuario?> ObtenerUsuarioPorId(Guid Id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:7181/api/users/getUsers");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var contenido = await response.Content.ReadAsStringAsync();
                Console.WriteLine(contenido);

                var dto = JsonSerializer.Deserialize<List<UsuarioDto>>(contenido, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dto == null)
                {
                    return null;
                }

                Usuario usuario= new Usuario();
                foreach (var user in dto)
                {

                    if (user.Id == Id)
                    {
                        usuario.Nombre = user.Nombre;
                        usuario.Email=user.Email;
                        
                    }
                    
                }

                if (usuario == null)
                {
                    return null;
                }
                return usuario;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

    }
}
