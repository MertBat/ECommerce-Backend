using ECommerce.Application.Constants;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Features.Commands.Basket.AddItemToBasket;
using ECommerce.Application.Features.Commands.Basket.RemoveBasketItem;
using ECommerce.Application.Features.Commands.Basket.UpdateQuantity;
using ECommerce.Application.Features.Queries.Basket.GetBasketItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class BasketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BasketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Baskets, ActionType = Application.Enums.ActionType.Read, Definition = "Get Basket Items")]
        public async Task<IActionResult> GetBasketItems([FromQuery] GetBasketItemsQueryRequest getBasketItemsQueryRequest)
        {
            List<GetBasketItemsQueryResponse> response = await _mediator.Send(getBasketItemsQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Baskets, ActionType = Application.Enums.ActionType.Write, Definition = "Add Item To Basket")]
        public async Task<IActionResult> AddItemToBasket(AddItemBasketCommandRequest addItemBasketCommandRequest)
        {
            AddItemBasketCommandResponse response = await _mediator.Send(addItemBasketCommandRequest);
            return Ok(response);
        }

        [HttpPut]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Baskets, ActionType = Application.Enums.ActionType.Update, Definition = "Update Quantity")]
        public async Task<IActionResult> UpdateQuantity(UpdateQuantityCommandRequest updateQuantityCommandRequest)
        {
            UpdateQuantityCommandResponse response = await _mediator.Send(updateQuantityCommandRequest);
            return Ok(response);
        }

        [HttpDelete("{BasketItemId}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Baskets, ActionType = Application.Enums.ActionType.Delete, Definition = "Remove Basket Item")]
        public async Task<IActionResult> RemoveItemInBasket([FromRoute] RemoveBasketItemCommandRequest removeBasketItemCommandRequest)
        {
            RemoveBasketItemCommandResponse response = await _mediator.Send(removeBasketItemCommandRequest);
            return Ok(response);
        }
    }
}
