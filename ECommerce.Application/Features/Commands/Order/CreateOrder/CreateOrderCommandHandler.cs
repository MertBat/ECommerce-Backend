using ECommerce.Application.Abstraction.Hubs;
using ECommerce.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Order.CreateOrder
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommandRequest, CreateOrderCommandResponse>
    {
        private readonly IOrderService _orderService;
        private readonly IBasketService _basketService;
        private readonly IOrderHubServices _orderHubServices;

        public CreateOrderCommandHandler(IOrderService orderService, IBasketService basketService, IOrderHubServices orderHubServices)
        {
            _orderService = orderService;
            _basketService = basketService;
            _orderHubServices = orderHubServices;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
        {
            string? basketId = _basketService.GetUserActiveBasket?.Id.ToString();
            if (basketId is null)
                throw new Exception("Couldn't find active basket");

           await _orderService.CreateOrderAsync( basketId, request.Address, request.Description);

            await _orderHubServices.OrderAddedMessageAsync("New order accepted");

            return new();
        }
    }
}
