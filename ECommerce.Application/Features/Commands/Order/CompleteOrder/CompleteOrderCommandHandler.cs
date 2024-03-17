using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs.Order;
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
        private readonly IMailService _mailService;

        public CompleteOrderCommandHandler(IOrderService orderService, IMailService mailService)
        {
            _orderService = orderService;
            _mailService = mailService;
        }
        public async Task<CompleteOrderCommandResponse> Handle(CompleteOrderCommandRequest request, CancellationToken cancellationToken)
        {
            (bool succeeded, CompletedOrderDTO completedOrderDTO) = await _orderService.ComleteOrderAsync(request.Id, request.OrderStatus);
            if (succeeded)
            {
                await _mailService.SendCompletedOrderMailAsync(
                    completedOrderDTO.Email,
                    completedOrderDTO.OrderCode,
                    completedOrderDTO.OrderDate,
                    completedOrderDTO.OrderStatus,
                    completedOrderDTO.Products,
                    completedOrderDTO.TotalPrice
                    );
            }
            return new();
        }
    }
}
