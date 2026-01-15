using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payments_services.application.Interfaces
{
    public interface IActivityService
    {
        Task<bool> RegisterActivityAsync(string email, string action, string category);
    }
}
