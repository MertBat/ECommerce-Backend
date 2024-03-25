using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.AuthorizationEndpoint.GetRolesEndpoint
{
    public class GetRolesEndpointQueryRequest : IRequest<GetRolesEndpointQueryResponse>
    {
        public string Code { get; set; }
    }
}
