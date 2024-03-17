using ECommerce.Application.DTOs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Order
{
    public class CompletedOrderDTO
    {
        public string Email { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public bool OrderStatus { get; set; }
        public List<MailListProductDTO> Products { get; set; }
        public long TotalPrice { get; set; }
    }
}
