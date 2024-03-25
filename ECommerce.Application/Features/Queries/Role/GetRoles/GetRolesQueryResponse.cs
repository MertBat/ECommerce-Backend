using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryResponse
    {
        public IDictionary<string,string> Roles { get; set; }
        public int TotalCount { get; set; }
    }
}
