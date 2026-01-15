using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Interfaces
{
    /// <summary>
    /// Clase interface que define las operaciones que se pueden realizar sobre usuarios, en el Microservicio Usuarios.
    /// </summary>
    public interface IUsuarioService
    {
        Task<Guid> ObtenerUsuarioPorEmailAsync(string correo);
        Task<Usuario> ObtenerUsuarioPorEmail(string email);
        Task<Usuario> ObtenerUsuarioPorId(Guid Id);

    }
}
