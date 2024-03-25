using ECommerce.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Product.UpdateProduct
{
    public class UpdateSingleProductCommandHandler : IRequestHandler<UpdateSingleProductCommandRequest, UpdateSingleProductCommandResponse>
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;

        public UpdateSingleProductCommandHandler(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }
        public async Task<UpdateSingleProductCommandResponse> Handle(UpdateSingleProductCommandRequest request, CancellationToken cancellationToken)
        {
            ECommerce.Domain.Entities.Product product = await _productReadRepository.GetById(request.Id);
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;
            await _productWriteRepository.saveAsync();

            return new();
        }
    }
}
