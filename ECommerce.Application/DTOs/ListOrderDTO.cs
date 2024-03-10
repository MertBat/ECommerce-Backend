using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs
{
    public class ListOrderDTO
    {
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public float TotalPrice { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
