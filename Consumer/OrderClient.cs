using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consumer;

public class OrderClient : IOrderClient
{
    private readonly Uri _uri;

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public OrderClient(Uri uri)
    {
        _uri = uri;
    }

    public async Task<List<Order>> GetOrders()
    {
        using (var client = new HttpClient())
        {
            var orders = await client.GetFromJsonAsync<List<Order>>($"{_uri}api/orders/GetOrders", Options);
            return orders;
        }
    }

    public async Task<Order> GetOrderById(int id)
    {
        using (var client = new HttpClient())
        {
            var order = await client.GetFromJsonAsync<Order>($"{_uri}api/orders/GetOrderById?id={id}", Options);
            return order;
        }
    }

    public int CreateOrder(string orderName)
    {
        throw new NotImplementedException();
    }
}