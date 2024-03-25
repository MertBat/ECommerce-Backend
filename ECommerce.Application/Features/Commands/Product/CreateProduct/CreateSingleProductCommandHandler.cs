using ECommerce.Application.Abstraction.Hubs;
using ECommerce.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Product.CreateProduct
{
    public class CreateSingleProductCommandHandler : IRequestHandler<CreateSingleProductCommandRequest, CreateSingleProductCommandResponse>
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductHubService _productHubService;

        public CreateSingleProductCommandHandler(IProductWriteRepository productWriteRepository, IProductHubService productHubService)
        {
            _productWriteRepository = productWriteRepository;
            _productHubService = productHubService;
        }

        public async Task<CreateSingleProductCommandResponse> Handle(CreateSingleProductCommandRequest request, CancellationToken cancellationToken)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name = request.Name,
                Price = Convert.ToInt64(request.Price),
                Stock = request.Stock
            });
            await _productWriteRepository.saveAsync();
            await _productHubService.ProductAddedMessageAsync($"{request.Name} isminde ürün eklenmiştir.");
            return new();
        }
    }
}
