using ECommerce.Application.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Role.GetAllRoles
{
    public class GetAllRolesQueryResponse
    {
        public List<GetAllRolesDTO> Roles { get; set; }
    }
}
