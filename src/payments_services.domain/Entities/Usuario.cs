using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace payments_services.domain.Entities
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Email { get; set; }

        [JsonConstructor]
        public Usuario(string nombre, string email)
        {
            Nombre = nombre;
            Email = email;
        }

        public Usuario(){}

    }
}
