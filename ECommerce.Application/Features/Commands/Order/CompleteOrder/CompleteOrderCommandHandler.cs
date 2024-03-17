using ECommerce.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Order.CompleteOrder
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommandRequest, CompleteOrderCommandResponse>
    {
        private readonly IOrderService _orderService;

        public CompleteOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<CompleteOrderCommandResponse> Handle(CompleteOrderCommandRequest request, CancellationToken cancellationToken)
        {
            await _orderService.ComleteOrderAsync(request.Id, request.OrderStatus);
            return new();
        }
    }
}
