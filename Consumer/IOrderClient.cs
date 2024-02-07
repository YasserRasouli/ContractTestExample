namespace Consumer;

public interface IOrderClient
{
    public Task<List<Order>> GetOrders();
    public Task<Order> GetOrderById(int id);
    public int CreateOrder(string orderName);
}