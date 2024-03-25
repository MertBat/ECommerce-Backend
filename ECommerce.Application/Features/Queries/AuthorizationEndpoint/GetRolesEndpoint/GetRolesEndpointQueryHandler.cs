using ECommerce.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.AuthorizationEndpoint.GetRolesEndpoint
{
    public class GetRolesEndpointQueryHandler : IRequestHandler<GetRolesEndpointQueryRequest, GetRolesEndpointQueryResponse>
    {
        private readonly IAuthorizationEndpointService _authorizationEndpointService;

        public GetRolesEndpointQueryHandler(IAuthorizationEndpointService authorizationEndpointService)
        {
            _authorizationEndpointService = authorizationEndpointService;
        }

        public async Task<GetRolesEndpointQueryResponse> Handle(GetRolesEndpointQueryRequest request, CancellationToken cancellationToken)
        {
            var roles = await _authorizationEndpointService.GetRolesToEndpointAsync(request.Code);

            return new()
            {
               Roles = roles,
            };
        }
    }
}
