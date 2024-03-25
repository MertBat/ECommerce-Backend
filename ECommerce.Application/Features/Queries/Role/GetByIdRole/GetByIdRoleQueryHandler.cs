using ECommerce.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Role.GetByIdRole
{
    public class GetByIdRoleQueryHandler : IRequestHandler<GetByIdRoleQueryRequest, GetByIdRoleQueryResponse>
    {
        private readonly IRoleService _roleService;

        public GetByIdRoleQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetByIdRoleQueryResponse> Handle(GetByIdRoleQueryRequest request, CancellationToken cancellationToken)
        {
           var data = await _roleService.GetRoleByIdAsync(request.Id);
            return new()
            {
                Id = data.id,
                Name = data.name
            };

        }
    }
}
