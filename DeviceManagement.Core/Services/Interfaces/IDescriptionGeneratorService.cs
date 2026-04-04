using DeviceManagement.Core.DTOs.Device;

namespace DeviceManagement.Core.Interfaces;

public interface IDescriptionGeneratorService
{
    Task<string> GenerateDescriptionAsync(GenerateDescriptionDto dto);
}