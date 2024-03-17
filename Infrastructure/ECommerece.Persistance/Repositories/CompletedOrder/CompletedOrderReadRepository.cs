using ECommerce.Application.Repositories.CompletedOrder;
using ECommerce.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Repositories.CompletedOrder
{
    public class CompletedOrderReadRepository : ReadRepository<Domain.Entities.CompletedOrder>, ICompletedOrderReadRepository
    {
        public CompletedOrderReadRepository(ECommerceAPIDbContext context) : base(context)
        {
        }
    }
}
