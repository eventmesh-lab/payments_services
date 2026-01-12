using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Interfaces
{
    public interface INotificationServices
    {
        Task EnviarPagoExitoso(string userId, string amount);
        Task ReservaConfirmadaExitos(string email);
    }
}
