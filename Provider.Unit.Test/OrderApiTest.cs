using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace Provider.Unit.Test;

public class OrderApiTest : IClassFixture<OrderApiFixture>
{
    private readonly OrderApiFixture _fixture;
    private readonly PactVerifier _pactVerifier;

    public OrderApiTest(OrderApiFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;

        var config = new PactVerifierConfig()
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new List<IOutput>
            {
                new XunitOutput(output)
            }
        };

        _pactVerifier = new PactVerifier(config);
    }

    [Fact]
    public void Verify()
    {
        // Arrange
        string pactPath = Path.Combine("..",
            "..",
            "..",
            "..",
            "Consumer.Unit.Test",
            "pacts",
            "My Consumer-My Provider.json");

        // Act / Assert
        _pactVerifier
            .ServiceProvider("My Provider", _fixture.ServerUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(_fixture.ServerUri, "/provider-states")).Verify();

        _pactVerifier.Dispose();
    }
}