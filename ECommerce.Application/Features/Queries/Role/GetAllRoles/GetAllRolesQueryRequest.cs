using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Role.GetAllRoles
{
    public class GetAllRolesQueryRequest : IRequest<GetAllRolesQueryResponse>
    {
    }
}
