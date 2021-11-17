using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderingService.Commands.CreateOrder;
using OrderingService.Commands.GetOrder;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Mediators;
using OrderingService.Domain.Orders;
using OrderingService.Domain.Transcoders;
using OrderingService.Infrastructure.Configurations;
using OrderingService.Infrastructure.Repositories;

namespace OrderingService.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection serviceCollection)
        {
            //Mediator
            serviceCollection.AddScoped<IMediator, Mediator>();

            //Command Handlers
            serviceCollection.AddScoped<ICommandHandler<CreateOrderCommand, string>, CreateOrderCommandHandler>();
            serviceCollection.AddScoped<ICommandHandler<GetOrderCommand, Order>, GetOrderCommandHandler>();

            return serviceCollection;
        }

        public static IServiceCollection AddTranscoders(this IServiceCollection serviceCollection)
        {
            //Top level transcoder
            serviceCollection.AddScoped<ITranscoder, Transcoder>();

            //Encoders
            serviceCollection.AddScoped<IEncoder, OrderEncoder>();
            serviceCollection.AddScoped<IEncoder, CreateOrderCommandEncoder>();

            //Decoders
            serviceCollection.AddScoped<IDecoder, OrderDecoder>();
            serviceCollection.AddScoped<IDecoder, CreateOrderCommandDecoder>();

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
