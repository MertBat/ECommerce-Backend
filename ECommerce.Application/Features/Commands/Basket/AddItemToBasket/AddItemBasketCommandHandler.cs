using ECommerce.Application.Abstraction.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Basket.AddItemToBasket
{
    public class AddItemBasketCommandHandler : IRequestHandler<AddItemBasketCommandRequest, AddItemBasketCommandResponse>
    {
        private readonly IBasketService _basketService;

        public AddItemBasketCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }
        public async Task<AddItemBasketCommandResponse> Handle(AddItemBasketCommandRequest request, CancellationToken cancellationToken)
        {
            await _basketService.AddItemToBasket(request.ProductId, request.Quantity);

            return new();
        }
    }
}
