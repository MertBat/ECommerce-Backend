using ECommerce.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class BasketItem : BaseEntity
    {
        public int Quantity { get; set; }

        //Navigation
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Basket Basket { get; set; }
    }
}
