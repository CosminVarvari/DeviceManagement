using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DeviceManagement.Core.DTOs.Device;
using DeviceManagement.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DeviceManagement.Infrastructure.Services;

public class DescriptionGeneratorService : IDescriptionGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DescriptionGeneratorService(IConfiguration config)
    {
        var apiKey = config["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq API key not configured.");

        _model = config["Groq:Model"] ?? "llama-3.1-8b-instant";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.groq.com/")
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> GenerateDescriptionAsync(GenerateDescriptionDto dto)
    {
        var requestBody = new
        {
            model = _model,
            max_tokens = 100,
            temperature = 0.7,
            messages = new[]
            {
                new
                {
                    role    = "system",
                    content = """
                        You are a concise technical writer for a corporate device management system.
                        Generate short, professional, and informative descriptions for mobile devices.

                        Rules:
                        - Maximum 2 sentences
                        - Focus on practical business use
                        - Mention the manufacturer, device type, and OS naturally
                        - Highlight standout specs only if relevant
                        - Never start with "This device"
                        - Return only the description, no extra text or formatting
                        """
                },
                new
                {
                    role    = "user",
                    content = BuildPrompt(dto)
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("openai/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Groq API error: {response.StatusCode} — {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<GroqResponse>(responseJson, JsonOptions);

        return parsed?.Choices?[0]?.Message?.Content?.Trim()
            ?? throw new InvalidOperationException("Empty response from Groq API.");
    }

    private static string BuildPrompt(GenerateDescriptionDto dto) =>
        $"""
        Generate a device description for the following specifications:

        - Name: {dto.Name}
        - Manufacturer: {dto.Manufacturer}
        - Type: {dto.Type}
        - Operating System: {dto.OperatingSystem} {dto.OsVersion}
        - Processor: {dto.Processor}
        - RAM: {dto.RamAmount}GB
        """;
    private record GroqResponse(List<GroqChoice>? Choices);
    private record GroqChoice(GroqMessage? Message);
    private record GroqMessage(string? Content);
}