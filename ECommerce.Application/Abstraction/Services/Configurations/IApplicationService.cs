using ECommerce.Application.DTOs.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Services.Configurations
{
    public interface IApplicationService
    {
        List<MenuDTO> GetAuthorizeDefinitionEndpoints(Type type);
    }
}
