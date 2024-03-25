using ECommerce.Application.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Services
{
    public interface IRoleService
    {
        IDictionary<string, string> GetRoles(int page, int size);
        List<GetAllRolesDTO> GetAllRoles();
        Task<(string id, string name)> GetRoleByIdAsync(string id);
        Task CreateRoleAsync(string name);
        Task DeleteRoleAsync(string id);
        Task UpdateRoleAsync(string id, string name);
        int GetTotalRoleCount();
    }
}
