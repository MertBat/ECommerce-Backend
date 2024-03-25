using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.Abstraction.Services.Configurations;
using ECommerce.Application.DTOs.AuthorizationEndpoint;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistance.Services
{
    public class AuthorizationEndpointService : IAuthorizationEndpointService
    {
        private readonly IApplicationService _applicationService;
        private readonly IEndpointReadRepository _endpointReadRepository;
        private readonly IEndpointWriteRepository _endpointWriteRepository;
        private readonly IMenuReadRepository _menuReadRepository;
        private readonly IMenuWriteRepository _menuWriteRepository;
        private readonly RoleManager<AppRole> _roleManager;

        public AuthorizationEndpointService(IApplicationService applicationService, IEndpointReadRepository endpointReadRepository, IEndpointWriteRepository endpointWriteRepository, IMenuReadRepository menuReadRepository, IMenuWriteRepository menuWriteRepository, RoleManager<AppRole> roleManager)
        {
            _applicationService = applicationService;
            _endpointReadRepository = endpointReadRepository;
            _endpointWriteRepository = endpointWriteRepository;
            _menuReadRepository = menuReadRepository;
            _menuWriteRepository = menuWriteRepository;
            _roleManager = roleManager;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string code, Type type, string menu)
        {
            Menu getMenu = await _menuReadRepository.GetSingleAsync(m => m.Name == menu);
            if (getMenu == null)
            {
                getMenu = new()
                {
                    Id = Guid.NewGuid(),
                    Name = menu,
                };

                await _menuWriteRepository.AddAsync(getMenu);
                await _menuWriteRepository.saveAsync();
            }

            Endpoint? endPoint = await _endpointReadRepository.Table.Include(e => e.Menu).Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);
            if (endPoint == null)
            {
                var action = _applicationService.GetAuthorizeDefinitionEndpoints(type).FirstOrDefault(m => m.Name == menu)?.Actions.FirstOrDefault(m => m.Code == code);

                endPoint = new()
                {
                    Code = action.Code,
                    ActionType = action.ActionType,
                    HttpType = action.HttpType,
                    Definition = action.Definition,
                    Id = Guid.NewGuid(),
                    Menu = getMenu,
                };

                await _endpointWriteRepository.AddAsync(endPoint);
                await _endpointWriteRepository.saveAsync();
            }

            var approles = await _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();

            foreach (var role in endPoint.Roles)
            {
                if (approles.FirstOrDefault(r => r.Id == role.Id) == null)
                    endPoint.Roles.Remove(role);
            }

            foreach (var role in approles)
            {
                if (endPoint.Roles.FirstOrDefault(r => r.Id == role.Id) == null)
                    endPoint.Roles.Add(role);
            }

            await _endpointWriteRepository.saveAsync();
        }

        public async Task<List<string>> GetRolesToEndpointAsync(string code)
        {
           Endpoint? endpoint = await _endpointReadRepository.Table.Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == code);

           return endpoint.Roles.Select(r => r.Name).ToList();
        }
    }
}
