using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace payments_services.application.DTOs
{
    public class UsuarioDto
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
        [JsonPropertyName("FullName")]
        public string Nombre { get; set; }
        [JsonPropertyName("Email")]
        public string Email { get; set; }
    }
}
