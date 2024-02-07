using System.Collections.Concurrent;

namespace Provider;

public class OrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<int, OrderDto> _orders = new();

    public Task<List<OrderDto>> GetAllAsync()
    {
        var orders = new List<OrderDto>();

        foreach (var item in _orders)
        {
            orders.Add(item.Value);
        }

        return Task.FromResult(orders);
    }

    public Task<OrderDto> GetAsync(int id)
    {
        var order = _orders[id];
        return Task.FromResult(order);
    }

    public Task InsertAsync(OrderDto order)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task DeleteAllAsync()
    {
        throw new NotImplementedException();
    }
}