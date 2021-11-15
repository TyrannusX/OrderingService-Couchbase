using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderingService.Commands.CreateOrder;
using OrderingService.Commands.GetOrder;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using OrderingService.Infrastructure.Configurations;
using OrderingService.Infrastructure.Repositories;

namespace OrderingService.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICommandHandler<CreateOrderCommand, string>, CreateOrderCommandHandler>();
            serviceCollection.AddScoped<ICommandHandler<GetOrderCommand, Order>, GetOrderCommandHandler>();
            return serviceCollection;
        }

        public static IServiceCollection AddTranscoders(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITranscoder<Order>, OrderTranscoder>();
            serviceCollection.AddScoped<ITranscoder<CreateOrderCommand>, CreateOrderCommandTranscoder>();
            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IRepository<Order>, CouchbaseOrderRepository>();
            return serviceCollection;
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            //Couchbase
            CouchbaseConfiguration couchbaseConfiguration = new CouchbaseConfiguration();
            couchbaseConfiguration = configuration.GetSection("CouchbaseConfiguration").Get<CouchbaseConfiguration>();
            serviceCollection.AddSingleton(couchbaseConfiguration);
            return serviceCollection;
        }
    }
}
