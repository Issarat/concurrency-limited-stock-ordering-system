using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LimitedStockOrdering.Api.Tests;

public sealed class SystemEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task HealthReturnsOk()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task StatusReturnsServiceMetadata()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/system/status");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("LimitedStockOrdering.Api", content, StringComparison.Ordinal);
    }
}
