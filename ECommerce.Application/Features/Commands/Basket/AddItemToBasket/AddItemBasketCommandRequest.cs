﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Basket.AddItemToBasket
{
    public class AddItemBasketCommandRequest : IRequest<AddItemBasketCommandResponse>
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
