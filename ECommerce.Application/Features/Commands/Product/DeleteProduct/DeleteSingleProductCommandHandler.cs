using ECommerce.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Product.DeleteProduct
{
    public class DeleteSingleProductCommandHandler : IRequestHandler<DeleteSingleProductCommandRequest, DeleteSingleProductCommandResponse>
    {
        private readonly IProductWriteRepository _productWriteRepository;

        public DeleteSingleProductCommandHandler(IProductWriteRepository productWriteRepository)
        {
            _productWriteRepository = productWriteRepository;
        }
        public async Task<DeleteSingleProductCommandResponse> Handle(DeleteSingleProductCommandRequest request, CancellationToken cancellationToken)
        {
            await _productWriteRepository.Remove(request.Id);
            await _productWriteRepository.saveAsync();
            return new();
        }
    }
}
