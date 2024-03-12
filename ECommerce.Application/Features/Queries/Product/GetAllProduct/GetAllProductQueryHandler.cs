using ECommerce.Application.Abstraction.Hubs;
using ECommerce.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly ILogger<GetAllProductQueryHandler> _logger;
        private readonly IProductHubService _productHubService;
        private readonly IConfiguration _configuration;

        public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<GetAllProductQueryHandler> logger, IProductHubService productHubService, IConfiguration configuration)
        {
            _productReadRepository = productReadRepository;
            _logger = logger;
            _productHubService = productHubService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Skip(request.Page * request.Size).Take(request.Size)
                .Include(p=> p.ProductImageFiles)
                .Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate,
                p.ProductImageFiles
            }).ToList();

            _logger.LogInformation("All products");
            await _productHubService.ProductAddedMessageAsync($"Ürünler getirilmiştir.");

            foreach (var product in products)
            {
                foreach (var productImageFile in product.ProductImageFiles)
                {
                    if (!string.IsNullOrEmpty(productImageFile.Path))
                    {
                        productImageFile.Path = _configuration["BaseStorageUrl"] + productImageFile.Path;
                    }
                }
            }

            return new() 
            { 
                Products = products, 
                TotalCount = totalCount 
            };
        }
    }
}
