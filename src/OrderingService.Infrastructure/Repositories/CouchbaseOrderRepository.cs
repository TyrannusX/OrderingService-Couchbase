using Couchbase;
using Couchbase.Core.IO.Transcoders;
using Couchbase.KeyValue;
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
        private readonly CouchbaseConfiguration _couchbaseConfiguration;
        private readonly ITranscoder<Order> _orderTranscoder;

        public CouchbaseOrderRepository(CouchbaseConfiguration couchbaseConfiguration, ITranscoder<Order> orderTranscoder)
        {
            _couchbaseConfiguration = couchbaseConfiguration;
            _orderTranscoder = orderTranscoder;
        }

        public async Task<string> Create(Order item)
        {
            ICouchbaseCollection collection = await GetCollection();

            byte[] encodedOrder = await _orderTranscoder.Encode(item);
            await collection.UpsertAsync(item.Id, encodedOrder, options => options.Transcoder(new RawJsonTranscoder()));

            return item.Id;
        }

        public async Task Delete(string id)
        {
            ICouchbaseCollection collection = await GetCollection();
            await collection.RemoveAsync(id);
        }

        public async Task<Order> Read(string id)
        {
            ICouchbaseCollection collection = await GetCollection();
            IGetResult couchbaseData = await collection.GetAsync(id, options => options.Transcoder(new RawJsonTranscoder()));
            byte[] encodedOrder = couchbaseData.ContentAs<byte[]>();
            Order order = await _orderTranscoder.Decode(encodedOrder);
            return order;
        }

        public async Task<Order> Update(Order item, string id)
        {
            ICouchbaseCollection collection = await GetCollection();
            byte[] encodedOrder = await _orderTranscoder.Encode(item);
            await collection.UpsertAsync(item.Id, encodedOrder, options => options.Transcoder(new RawJsonTranscoder()));
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
