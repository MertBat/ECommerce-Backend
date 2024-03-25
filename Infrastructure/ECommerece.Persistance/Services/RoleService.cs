using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs.Role;
using ECommerce.Application.Exceptions;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task CreateRoleAsync(string name)
        {
            AppRole role = await _roleManager.FindByNameAsync(name);
            if (role == null)
            {
                IdentityResult result = await _roleManager.CreateAsync(new() { Id = Guid.NewGuid().ToString(), Name = name });
                if (!result.Succeeded)
                {
                    throw new RoleException();
                }
            }
            else
                throw new RoleException($"{name} role exist");
        }

        public async Task DeleteRoleAsync(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new RoleException();
                }
            }
            else
                throw new RoleException($"Role is not exist");
        }

        public IDictionary<string, string> GetRoles(int page, int size)
        {
            return _roleManager.Roles.Skip(page * size).Take(size).ToDictionary(role => role.Id, role => role.Name);
        }

        public async Task<(string id, string name)> GetRoleByIdAsync(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            return (role.Id, role.Name);
        }

        public int GetTotalRoleCount()
        {
            return _roleManager.Roles.ToList().Count;
        }

        public async Task UpdateRoleAsync(string id, string name)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                role.Name = name;
                IdentityResult result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    throw new RoleException();
                }
            }
            else
                throw new RoleException();
        }

        public List<GetAllRolesDTO> GetAllRoles()
        {
            return _roleManager.Roles.Select(r => new GetAllRolesDTO
            {
                Name = r.Name,
                Id = r.Id
            }).ToList();
        }
    }
}
