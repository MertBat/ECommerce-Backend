using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Order.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQueryRequest, GetAllOrdersQueryResponse>
    {
        private readonly IOrderService _orderService;

        public GetAllOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetAllOrdersQueryResponse> Handle(GetAllOrdersQueryRequest request, CancellationToken cancellationToken)
        {
            List<ListOrderDTO> listOrders = await _orderService.GetAllOrdersAsync(request.Page, request.Size);

            int totalOrdeCount = await _orderService.GetAllOrdersCountAsync();

            return new GetAllOrdersQueryResponse
            {
                Orders = listOrders,
                TotalOrderCount = totalOrdeCount
            };
        }
    }
}
