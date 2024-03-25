using ECommerce.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public string Name { get; set; }

        //Navigation
        public ICollection<Endpoint> Endpoints { get; set; }
    }
}
