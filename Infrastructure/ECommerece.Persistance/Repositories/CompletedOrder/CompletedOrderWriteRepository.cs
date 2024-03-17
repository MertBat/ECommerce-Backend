using ECommerce.Application.Repositories.CompletedOrder;
using ECommerce.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Repositories.CompletedOrder
{
    public class CompletedOrderWriteRepository : WriteRepository<Domain.Entities.CompletedOrder>, ICompletedOrderWriteRepository
    {
        public CompletedOrderWriteRepository(ECommerceAPIDbContext context) : base(context)
        {
        }
    }
}
