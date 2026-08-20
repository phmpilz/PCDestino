using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PCDestino.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class ApiTests(ApiFactory factory)
{
    [Fact]
    public async Task Readiness_WhenDatabaseIsAvailable_ReturnsOk()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Catalog_ReturnsSeededCity()
    {
        using var client = factory.CreateClient();

        var cities = await client.GetFromJsonAsync<JsonElement>("/api/v1/catalog/cities");

        Assert.Equal(JsonValueKind.Array, cities.ValueKind);
        Assert.Contains(cities.EnumerateArray(), city => city.GetProperty("slug").GetString() == "campinas-sp");
    }

    [Fact]
    public async Task Places_ReturnsPublishedSeedData()
    {
        using var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<JsonElement>("/api/v1/places?page=1&pageSize=20");

        Assert.True(page.GetProperty("total").GetInt32() >= 2);
        Assert.NotEmpty(page.GetProperty("items").EnumerateArray());
    }

    [Fact]
    public async Task Profile_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Profile_WithDevelopmentAuthentication_ReturnsUser()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Dev-User-Id", "integration-user");
        client.DefaultRequestHeaders.Add("X-Dev-User-Name", "Pessoa de Teste");

        var profile = await client.GetFromJsonAsync<JsonElement>("/api/v1/me");

        Assert.Equal("integration-user", profile.GetProperty("userId").GetString());
        Assert.Equal("Pessoa de Teste", profile.GetProperty("displayName").GetString());
    }
}
