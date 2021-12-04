using Couchbase;
using Couchbase.Core.Exceptions;
using Couchbase.Core.IO.Transcoders;
using Couchbase.KeyValue;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using OrderingService.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Infrastructure.Repositories
{
    public class CouchbaseOrderRepository : IRepository<Order>
    {
        private readonly ILogger<CouchbaseOrderRepository> _logger;
        private readonly CouchbaseConfiguration _couchbaseConfiguration;
        private readonly ITranscoder _transcoder;

        public CouchbaseOrderRepository(ILogger<CouchbaseOrderRepository> logger, CouchbaseConfiguration couchbaseConfiguration, ITranscoder transcoder)
        {
            _logger = logger;
            _couchbaseConfiguration = couchbaseConfiguration;
            _transcoder = transcoder;
        }

        public async Task<string> Create(Order item)
        {
            ICouchbaseCollection collection = await GetCollection();

            byte[] encodedOrder = await _transcoder.Encode(item, nameof(Order));
            await collection.UpsertAsync(item.Id, encodedOrder, options => options.Transcoder(new RawJsonTranscoder()));

            return item.Id;
        }

        public async Task Delete(string id)
        {
            int counter = 0;
            while (counter < _couchbaseConfiguration.MaxCasRetries)
            {
                ICouchbaseCollection collection = await GetCollection();

                IGetResult couchbaseData = await collection.GetAsync(id, options => options.Transcoder(new RawJsonTranscoder()));

                try
                {
                    await collection.RemoveAsync(id, options =>
                    {
                        options.Cas(couchbaseData.Cas);
                    });
                    break;
                }
                catch (CasMismatchException)
                {
                    _logger.LogWarning($"Failed to delete record with id {id}. Retrying . . .");
                    counter++;
                }
            }

            if (counter == _couchbaseConfiguration.MaxCasRetries)
            {
                _logger.LogError($"Exceeded retries while attempting to delete record with id {id}");
                throw new ArgumentException();
            }
        }

        public async Task<Order> Read(string id)
        {
            ICouchbaseCollection collection = await GetCollection();
            IGetResult couchbaseData = await collection.GetAsync(id, options => options.Transcoder(new RawJsonTranscoder()));
            byte[] encodedOrder = couchbaseData.ContentAs<byte[]>();
            Order order = await _transcoder.Decode(encodedOrder, nameof(Order)) as Order;
            return order;
        }

        public async Task<Order> Update(Order item, string id)
        {
            int counter = 0;
            while (counter < _couchbaseConfiguration.MaxCasRetries)
            {
                ICouchbaseCollection collection = await GetCollection();

                IGetResult couchbaseData = await collection.GetAsync(id, options => options.Transcoder(new RawJsonTranscoder()));
                byte[] currentPersistedOrderAsBytes = couchbaseData.ContentAs<byte[]>();
                Order currentPersistedOrder = await _transcoder.Decode(currentPersistedOrderAsBytes, nameof(Order)) as Order;

                currentPersistedOrder.CustomerLastName = item.CustomerFirstName;
                currentPersistedOrder.CustomerLastName = item.CustomerLastName;
                currentPersistedOrder.Address.City = item.Address.City;
                currentPersistedOrder.Address.State = item.Address.State;
                currentPersistedOrder.Address.StreetName = item.Address.StreetName;
                currentPersistedOrder.Address.PostalCode = item.Address.PostalCode;
                currentPersistedOrder.Price = item.Price;

                byte[] encodedOrder = await _transcoder.Encode(item, nameof(Order));

                try
                {
                    await collection.ReplaceAsync(item.Id, encodedOrder, options =>
                    {
                        options.Transcoder(new RawJsonTranscoder());
                        options.Cas(couchbaseData.Cas);
                    });
                    break;
                }
                catch (CasMismatchException)
                {
                    _logger.LogWarning($"Failed to update record with id {id}. Retrying . . .");
                    counter++;
                }
            }

            if (counter == _couchbaseConfiguration.MaxCasRetries)
            {
                _logger.LogError($"Exceeded retries while attempting to update record with id {id}");
                throw new ArgumentException();
            }

            return item;
        }

        private async Task<ICouchbaseCollection> GetCollection()
        {
            ICluster cluster = await Cluster.ConnectAsync(_couchbaseConfiguration.ConnectionString, _couchbaseConfiguration.UserName, _couchbaseConfiguration.Password);
            IBucket bucket = await cluster.BucketAsync(_couchbaseConfiguration.OrdersBucketName);
            IScope scope = await bucket.ScopeAsync(_couchbaseConfiguration.OrdersBucketScope);
            return await scope.CollectionAsync(_couchbaseConfiguration.OrdersCollectionName);
        }
    }
}
