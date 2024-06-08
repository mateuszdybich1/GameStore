using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoOrderRepository : IOrderRepository
{
    private readonly IMongoCollection<MongoOrder> _orderCollection;
    private readonly IMongoCollection<MongoCustomer> _customerCollection;

    public MongoOrderRepository(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _orderCollection = database.GetCollection<MongoOrder>("orders");
        _customerCollection = database.GetCollection<MongoCustomer>("customers");
    }

    public Task Add(Order entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Order entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Order> Get(Guid id)
    {
        if (_orderCollection != null && _orderCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            var mongoOrder = await _orderCollection.Find(x => x.Id == id.AsObjectId()).FirstOrDefaultAsync();
            var orderCustomer = await _customerCollection.Find(x => x.CustomerID == mongoOrder.CustomerID).FirstOrDefaultAsync();
            return mongoOrder != null ? new Order(mongoOrder.Id.AsGuid(), orderCustomer.Id.AsGuid(), OrderStatus.Paid) : null;
        }
        else
        {
            return null;
        }
    }

    public async Task<IEnumerable<Order>> GetAllOrders(DateTime startDate, DateTime dateTo)
    {
        if (_customerCollection != null && _customerCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            var filter = new BsonDocument
        {
            {
                "OrderDate", new BsonDocument
                {
                    { "$gte", startDate.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                    { "$lte", dateTo.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                }
            },
        };

            var mongoOrders = await _orderCollection.Find(filter).ToListAsync();

            var orders = new List<Order>();

            foreach (var mongoOrder in mongoOrders)
            {
                var customer = await _customerCollection.Find(x => x.CustomerID == mongoOrder.CustomerID).FirstOrDefaultAsync();

                if (customer != null)
                {
                    orders.Add(new Order(mongoOrder.Id.AsGuid(), customer.Id.AsGuid(), DateTime.Parse(mongoOrder.OrderDate), OrderStatus.Paid));
                }
            }

            return orders;
        }
        else
        {
            return [];
        }
    }

    public Task<Order> GetCustomerOpenOrder(Guid customerId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId)
    {
        if (_customerCollection != null && _customerCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            var orderCustomer = await _customerCollection.Find(x => x.Id == customerId.AsObjectId()).FirstOrDefaultAsync();
            var orders = new List<Order>();

            if (orderCustomer != null)
            {
                var mongoOrders = await _orderCollection.Find(x => x.CustomerID == orderCustomer.CustomerID).ToListAsync();

                mongoOrders.ForEach(x => orders.Add(new Order(x.Id.AsGuid(), orderCustomer.Id.AsGuid(), OrderStatus.Paid)));
            }

            return orders;
        }
        else
        {
            return [];
        }
    }

    public async Task<IEnumerable<Order>> GetPaidAndCancelledOrders()
    {
        try
        {
            return await GetAllOrders(DateTime.MinValue, DateTime.Now);
        }
        catch
        {
            return [];
        }
    }

    public Task Update(Order entity)
    {
        throw new NotImplementedException();
    }
}
