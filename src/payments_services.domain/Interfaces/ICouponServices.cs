using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.domain.Interfaces
{
    public interface ICouponServices
    {
        Task MarcarCuponComoUsado(Guid id);
    }
}
