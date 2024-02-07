using Microsoft.AspNetCore.Mvc;

namespace Provider.Controllers;

[ApiController]
[Route("/api/orders/[action]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orders;

    public OrderController(IOrderRepository orders)
    {
        _orders = orders;
    }

    [HttpGet(Name = "GetOrders")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orders.GetAllAsync();
        return Ok(orders);
        //return Ok(new OrderDto() { Id = 5, Name = "abc" });
    }

    [HttpGet(Name = "GetOrderById")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var order = await _orders.GetAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost(Name = "CreateOrder")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrder(string orderName)
    {
        var generatedId = new Random().Next();
        var order = new OrderDto()
        {
            Id = generatedId,
            Name = orderName
        };

        await _orders.InsertAsync(order);
        return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, order);
    }
}