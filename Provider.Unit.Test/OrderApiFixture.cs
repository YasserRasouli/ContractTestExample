using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Provider.Unit.Test;

public class OrderApiFixture : IDisposable
{
    private readonly IHost server;
    public Uri ServerUri { get; }

    private readonly Startup inner;

    public OrderApiFixture()
    {
        ServerUri = new Uri("http://localhost:9223");
        server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(ServerUri.ToString());
                webBuilder.ConfigureServices(collection => collection.TryAddSingleton<IOrderRepository, FakeOrderRepository>());
                webBuilder.UseStartup<Startup>();
                // webBuilder.Configure(builder =>
                // {
                //     builder.UseMiddleware<ProviderStateMiddleware>();
                //     
                // });
            })
            .Build();
        server.Start();
    }

    public void Dispose()
    {
        server.Dispose();
    }
}