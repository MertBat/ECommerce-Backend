﻿using ECommerce.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Order.GetAllOrders
{
    public class GetAllOrdersQueryResponse
    {
        public List<ListOrderDTO> Orders { get; set; }
        public int TotalOrderCount { get; set; }
    }
}
