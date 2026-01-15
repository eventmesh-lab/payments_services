using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using payments_services.application.DTOs;

namespace payments_services.application.Interfaces
{
    public interface INotificationServices
    {
        Task EnviarPagoExitoso(string userId, string amount);
        Task ReservaConfirmadaExitos(string email);
        Task EnviarCorreoPagoExitosoDetallado(EnviarCorreoPagoExitosoDto dto);
    }
}
