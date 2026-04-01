using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using DeviceManagement.Tests;
using System.Text.Json;

namespace DeviceManagement.Tests.Controllers;

public class DevicesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    public DevicesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateAuthenticatedClient() => _factory.CreateClient();

    private async Task<HttpClient> GetAuthenticatedClientAsync(string email = "test@company.com")
    {
        var client = _factory.CreateClient();
        var token = await AuthHelper.GetTokenAsync(client, email);
        client.AddAuthHeader(token);
        return client;
    }


    [Fact]
    public async Task GetAll_WithoutAuth_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/devices");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithAuth_Returns200WithDevices()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/devices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var devices = await response.Content
            .ReadFromJsonAsync<List<Dictionary<string, JsonElement>>>();

        devices.Should().NotBeNullOrEmpty();
        devices!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_WithValidId_Returns200()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/devices/{TestConstants.DeviceId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var device = await response.Content
            .ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        device.Should().NotBeNull();
        device!["manufacturer"].GetString().Should().Be("Apple");
        device["id"].GetString().Should().Be(TestConstants.DeviceId.ToString());
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/devices/{TestConstants.NonExistentId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var client = await GetAuthenticatedClientAsync();
        var dto = new
        {
            name = "Pixel 9 Pro",
            manufacturer = "Google",
            type = "Phone",
            operatingSystem = "Android",
            osVersion = "15.0",
            processor = "Google Tensor G4",
            ramAmount = 16,
            description = "Latest Google flagship"
        };

        var response = await client.PostAsJsonAsync("/api/devices", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithDuplicateDevice_Returns409()
    {
        var client = await GetAuthenticatedClientAsync();
        var dto = new
        {
            name = "iPhone 15 Pro",
            manufacturer = "Apple",
            type = "Phone",
            operatingSystem = "iOS",
            osVersion = "17.4",
            processor = "A17 Pro",
            ramAmount = 8,
            description = "Duplicate"
        };

        var response = await client.PostAsJsonAsync("/api/devices", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_WithInvalidType_Returns400()
    {
        var client = await GetAuthenticatedClientAsync();
        var dto = new
        {
            name = "Unknown",
            manufacturer = "Unknown",
            type = "Laptop",
            operatingSystem = "Windows",
            osVersion = "11",
            processor = "Intel i7",
            ramAmount = 16,
            description = "Fail"
        };

        var response = await client.PostAsJsonAsync("/api/devices", dto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithMissingFields_Returns400()
    {
        var client = await GetAuthenticatedClientAsync();
        var dto = new
        {
            name = "",
            manufacturer = "Apple",
            type = "Phone",
            operatingSystem = "iOS",
            osVersion = "17.4",
            processor = "A17 Pro",
            ramAmount = 8
        };

        var response = await client.PostAsJsonAsync("/api/devices", dto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WithValidData_Returns200()
    {
        var client = await GetAuthenticatedClientAsync();
        var dto = new
        {
            name = "iPhone 15 Pro Updated",
            manufacturer = "Apple",
            type = "Phone",
            operatingSystem = "iOS",
            osVersion = "17.5",
            processor = "A17 Pro",
            ramAmount = 8,
            description = "Updated description"
        };

        var response = await client.PutAsJsonAsync($"/api/devices/{TestConstants.DeviceId}", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content
            .ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        updated!["osVersion"].GetString().Should().Be("17.5");
        updated["description"].GetString().Should().Be("Updated description");
    }

    [Fact]
    public async Task Update_WithNonExistentId_Returns404()
    {
        var client = await GetAuthenticatedClientAsync();
        var dto = new
        {
            name = "Ghost",
            manufacturer = "Ghost",
            type = "Phone",
            operatingSystem = "Android",
            osVersion = "14",
            processor = "Unknown",
            ramAmount = 4,
            description = "404"
        };

        var response = await client.PutAsJsonAsync($"/api/devices/{TestConstants.NonExistentId}", dto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_Returns204()
    {
        var client = await GetAuthenticatedClientAsync();
        var createDto = new
        {
            name = "Device To Delete",
            manufacturer = "Test Brand",
            type = "Tablet",
            operatingSystem = "Android",
            osVersion = "14.0",
            processor = "Snapdragon 700",
            ramAmount = 6,
            description = "Will be deleted"
        };

        var createResponse = await client.PostAsJsonAsync("/api/devices", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content
            .ReadFromJsonAsync<Dictionary<string, JsonElement>>();
        var id = created!["id"].GetString();

        var deleteResponse = await client.DeleteAsync($"/api/devices/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/devices/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/devices/{TestConstants.NonExistentId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Assign_FreeDevice_Returns200()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsync(
            $"/api/devices/{TestConstants.DeviceId}/assign", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var device = await response.Content
            .ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        device!["assignedUserId"].ValueKind.Should().NotBe(JsonValueKind.Null);
    }

    [Fact]
    public async Task Assign_AlreadyAssignedDevice_Returns409()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsync(
            $"/api/devices/{TestConstants.AssignedDeviceId}/assign", null);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Unassign_OwnDevice_Returns200()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsync(
            $"/api/devices/{TestConstants.AssignedDeviceId}/unassign", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var device = await response.Content
            .ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        device!["assignedUserId"].ValueKind.Should().Be(JsonValueKind.Null);
    }
}