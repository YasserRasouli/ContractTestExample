namespace Provider;

public interface IOrderRepository
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto> GetAsync(int id);
    Task InsertAsync(OrderDto order);
    Task DeleteAllAsync();
}