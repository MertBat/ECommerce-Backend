using ECommerce.Application.Abstraction.Storage;
using ECommerce.Infastructure.Services.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStorageService, StorageService>();
        }

        public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : Storage, IStorage
        {
            serviceCollection.AddScoped<IStorage, T>();
        }

        //Bu şekilde de dinamik bir yapı kurulabilinir.
        //public static void AddStorage>(this IServiceCollection serviceCollection, StorageType storageType) 
        //{
        //    serviceCollection.AddScoped<IStorage, T>();
        //}
    }

        //public enum StorageType
        //{
        //    Local, Azure, AWS
        //}
}
