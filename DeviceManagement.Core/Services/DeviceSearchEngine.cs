using DeviceManagement.Core.DTOs.Device;
using DeviceManagement.Infrastructure.Entities;
using System.Text.RegularExpressions;

namespace DeviceManagement.Core.Services;

public static class DeviceSearchEngine
{
    private const int NameScore = 40;
    private const int ManufacturerScore = 30;
    private const int ProcessorScore = 20;
    private const int RamScore = 10;
    private const int ExactMatchMultiplier = 2;

    public static IEnumerable<DeviceSearchResultDto> Search(
        IEnumerable<Device> devices,
        IEnumerable<DeviceDto> deviceDtos,
        string query)
    {
        var tokens = Tokenize(query);

        if (!tokens.Any())
            return Enumerable.Empty<DeviceSearchResultDto>();

        var dtoMap = deviceDtos.ToDictionary(d => d.Id);

        return devices
            .Select(device => new
            {
                Dto = dtoMap[device.Id],
                Score = CalculateScore(device, tokens)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Select(x => new DeviceSearchResultDto
            {
                Device = x.Dto,
                Score = x.Score
            });
    }

    private static int CalculateScore(Device device, IReadOnlyList<string> tokens)
    {
        var score = 0;

        foreach (var token in tokens)
        {
            score += ScoreField(device.Name, token, NameScore);
            score += ScoreField(device.Manufacturer, token, ManufacturerScore);
            score += ScoreField(device.Processor, token, ProcessorScore);
            score += ScoreRam(device.RamAmount, token);
        }

        return score;
    }

    private static int ScoreField(string fieldValue, string token, int baseScore)
    {
        if (string.IsNullOrEmpty(fieldValue)) return 0;

        var normalized = NormalizeText(fieldValue);
        if (!normalized.Contains(token)) return 0;

        var isWholeWord = Regex.IsMatch(normalized, $@"\b{Regex.Escape(token)}\b");
        return isWholeWord ? baseScore * ExactMatchMultiplier : baseScore;
    }

    private static int ScoreRam(int ramAmount, string token)
    {
        var numberMatch = Regex.Match(token, @"\d+");
        if (!numberMatch.Success) 
            return 0;

        if (!int.TryParse(numberMatch.Value, out var tokenRam)) 
            return 0;

        return ramAmount == tokenRam
            ? RamScore * ExactMatchMultiplier
            : 0;
    }

    public static IReadOnlyList<string> Tokenize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<string>();

        var lower = input.ToLowerInvariant();
        var cleaned = Regex.Replace(lower, @"[^\w\s]", " ");
        var separated = Regex.Replace(cleaned, @"(\d+)([a-z])", "$1 $2");
        separated = Regex.Replace(separated, @"([a-z])(\d+)", "$1 $2");
        var normalized = Regex.Replace(separated, @"\s+", " ").Trim();

        var stopWords = new HashSet<string> { "ram", "gb", "the", "and", "for" };

        return normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t =>
                !stopWords.Contains(t) &&
                (t.Length >= 2 || Regex.IsMatch(t, @"^\d+$")))
            .Distinct()
            .ToList();
    }

    private static string NormalizeText(string input)
    {
        var lower = input.ToLowerInvariant().Trim();
        var cleaned = Regex.Replace(lower, @"[^\w\s]", " ");

        var separated = Regex.Replace(cleaned, @"(\d+)([a-z])", "$1 $2");
        separated = Regex.Replace(separated, @"([a-z])(\d+)", "$1 $2");

        return Regex.Replace(separated, @"\s+", " ").Trim();
    }
}