﻿
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Application.Repositories;
using ECommerce.Persistance.Repositories;
using ECommerce.Application.Repositories.InvoiceFile;
using ECommerce.Persistance.Repositories.InvoiceFile;
using ECommerce.Persistance.Repositories.ProductImageFile;
using ECommerce.Application.Repositories.ProductImageFile;
using ECommerce.Persistance.Repositories.File;
using ECommerce.Application.Repositories.File;
using ECommerce.Application.Abstraction.Services;
using ECommerce.Persistance.Services;
using ECommerce.Application.Abstraction.Services.Authentications;
using ECommerce.Application.Repositories.BasketItem;
using ECommerce.Persistance.Repositories.BasketItem;
using ECommerce.Application.Repositories.Basket;
using ECommerce.Persistance.Repositories.Basket;

namespace ECommerce.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IInvoiceFileReadRepository, InvoiceFileReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository, InvoiceWriteRepository>();
            services.AddScoped<IProductImageFileReadRepositroy, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();
            services.AddScoped<IBasketReadRepository, BasketReadRepository>();
            services.AddScoped<IBasketWriteRepository, BasketWriteRepository>();
            services.AddScoped<IBasketItemReadRespository, BasketItemReadRepository>();
            services.AddScoped<IBasketItemWriteRepository, BasketItemWriteRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthentication, AuthService>();
            services.AddScoped<IInternalAuthentication, AuthService>();
            services.AddScoped<IBasketService, BasketService>();

        }
    }
}
