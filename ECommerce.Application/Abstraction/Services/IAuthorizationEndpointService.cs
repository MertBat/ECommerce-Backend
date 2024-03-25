using ECommerce.Application.DTOs.AuthorizationEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Services
{
    public interface IAuthorizationEndpointService
    {
        Task AssignRoleEndpointAsync(string[] roles, string code, Type type, string menu);
        Task<List<string>> GetRolesToEndpointAsync(string code);
    }
}
