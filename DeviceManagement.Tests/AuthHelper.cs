using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeviceManagement.Tests;

public static class AuthHelper
{
    public static async Task<string> GetTokenAsync(
        HttpClient client,
        string email = "test@company.com",
        string password = "Password123!")
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed ({response.StatusCode}): {error}");
        }

        var body = await response.Content
            .ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        return body!["token"].GetString()
            ?? throw new Exception("Token not found in login response.");
    }

    public static void AddAuthHeader(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}