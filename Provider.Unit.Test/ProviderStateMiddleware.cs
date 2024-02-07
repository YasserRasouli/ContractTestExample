using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Provider.Unit.Test;

public class ProviderStateMiddleware
{
    private readonly IOrderRepository _orders;
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _providerStates;

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ProviderStateMiddleware(RequestDelegate next, IOrderRepository orders)
    {
        _next = next;
        _orders = orders;
        _providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
        {
            [@"an order with ID=2 exists"] = CreateOrder,
            [@"three orders exist"] = CreateThreeOrders
        };
    }

    private async Task CreateOrder(IDictionary<string, object> parameters)
    {
        // var id = (Int64)parameters["id"];
        // var x = (Int32)id;
        // await _orders.InsertAsync(new OrderDto()
        // {
        //     Id = (Int32)id,
        //     Name = "laptop"
        // });
        //
        // var ords = await _orders.GetAllAsync();
        // var sdf = true;
    }

    private async Task CreateThreeOrders(IDictionary<string, object> parameters)
    {
        await _orders.DeleteAllAsync();
        await _orders.InsertAsync(new OrderDto()
        {
            Id = 4,
            Name = $"random-order-4"
        });
        await _orders.InsertAsync(new OrderDto()
        {
            Id = 6,
            Name = $"random-order-6"
        });
        await _orders.InsertAsync(new OrderDto()
        {
            Id = 8,
            Name = $"random-order-8"
        });
    }


    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/provider-states"))
        {
            await HandleProviderStatesRequest(context);
            await context.Response.WriteAsync(string.Empty);
        }
        else
        {
            await _next.Invoke(context);
        }
    }

    private async Task HandleProviderStatesRequest(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper())
        {
            string jsonRequestBody;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                jsonRequestBody = await reader.ReadToEndAsync();
            }

            try
            {
                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);
                var providerStateParams = JsonConvert.DeserializeObject<IDictionary<string, object>>(providerState.Params.ToString());

                //A null or empty provider state key must be handled
                if (!string.IsNullOrEmpty(providerState.State))
                {
                    await _providerStates[providerState.State].Invoke(providerStateParams);
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Failed to deserialise JSON provider state body:");
                await context.Response.WriteAsync(jsonRequestBody);
                await context.Response.WriteAsync(string.Empty);
                await context.Response.WriteAsync(e.ToString());
            }
        }
    }
}