using System;
using System.Collections.Generic;
using System.Text;

namespace OrderingService.Infrastructure.Configurations
{
    public class CouchbaseConfiguration
    {
        public string ConnectionString { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string OrdersBucketName { get; set; }
        public string OrdersBucketScope { get; set; }
        public string OrdersCollectionName { get; set; }
        public int MaxCasRetries { get; set; }
    }
}
