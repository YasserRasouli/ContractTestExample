using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using Xunit.Abstractions;

namespace Consumer.Unit.Test;

public class OrderTest
{
    private readonly IPactBuilderV3 _pactBuilder;
    private readonly OrderClient _orderClient;

    public OrderTest(ITestOutputHelper output)
    {
        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new[]
            {
                new XunitOutput(output)
            },
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        // you select which specification version you wish to use by calling either V2 or V3
        var pactV3 = Pact.V3("My Consumer", "My Provider", config);

        // the pact builder is created in the constructor so it's unique to each test
        _pactBuilder = pactV3.WithHttpInteractions();
    }

    [Fact]
    public async Task GetOrderById_WhenCalled_ReturnsDesiredOrder()
    {
        var expected = new Order()
        {
            Id = 2,
            Name = "laptop"
        };
    
        // Arrange
        _pactBuilder.UponReceiving("A valid request for an specific order")
            .Given("an order with ID=2 exists", new Dictionary<string, string> { ["id"] = "2" })
            .WithRequest(HttpMethod.Get, "/api/orders/GetOrderById")
            .WithQuery("id", "2")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new { Id = Match.Type(expected.Id), Name = Match.Type(expected.Name) });
    
        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var orderClient = new OrderClient(ctx.MockServerUri);
            var response = await orderClient.GetOrderById(2);
            Assert.IsType<string>(response.Name);
            Assert.IsType<int>(response.Id);
        });
    }

    [Fact]
    public async Task GetOrders_WhenCalled_GetAllOrders()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A valid request for all orders")
            .Given("three orders exist")
            .WithRequest(HttpMethod.Get, "/api/orders/GetOrders")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new List<object>()
            {
                new
                {
                    Id = Match.Type(new Random().Next()),
                    Name = Match.Type(new Random().Next().ToString())
                },
                new
                {
                    Id = Match.Type(new Random().Next()),
                    Name = Match.Type(new Random().Next().ToString())
                },
                new
                {
                    Id = Match.Type(new Random().Next()),
                    Name = Match.Type(new Random().Next().ToString())
                }
            });

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var orderClient = new OrderClient(ctx.MockServerUri);
            var response = await orderClient.GetOrders();

            for (int i = 0; i < response.Count; i++)
            {
                Assert.IsType<int>(response[i].Id);
                Assert.IsType<string>(response[i].Name);
            }
        });
    }
}