using ECommerce.Application.Repositories.InvoiceFile;
using ECommerce.Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Repositories.InvoiceFile
{
    public class InvoiceWriteRepository : WriteRepository<Domain.Entities.InvoiceFile>, IInvoiceFileWriteRepository
    {
        public InvoiceWriteRepository(ECommerceAPIDbContext context) : base(context)
        {
        }
    }
}
