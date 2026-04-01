using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace DeviceManagement.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var dto = new
        {
            name = "New User",
            email = "newuser@company.com",
            password = "Password123!",
            location = "Bucuresti"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var dto = new
        {
            name = "Duplicate",
            email = "test@company.com",
            password = "Password123!",
            location = "Cluj"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_Returns400()
    {
        var dto = new
        {
            name = "Test",
            email = "not-an-email",
            password = "Password123!",
            location = "Cluj"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400()
    {
        var dto = new
        {
            name = "Test",
            email = "weakpass@company.com",
            password = "weak",
            location = "Cluj"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        var dto = new
        {
            email = "test@company.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
        body.Should().ContainKey("token");
        body!["token"].ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var dto = new
        {
            email = "test@company.com",
            password = "WrongPassword!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns401()
    {
        var dto = new
        {
            email = "ghost@company.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}