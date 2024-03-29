﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Product.DeleteProduct
{
    public class DeleteSingleProductCommandRequest : IRequest<DeleteSingleProductCommandResponse>
    {
        public string Id { get; set; }
    }
}
