using DeviceManagement.Core.DTOs.Device;

namespace DeviceManagement.Core.Services.Interfaces;

public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<DeviceDto> GetByIdAsync(Guid id);
    Task<DeviceDto> CreateAsync(DeviceRequestDto dto);
    Task<DeviceDto> UpdateAsync(Guid id, DeviceRequestDto dto);
    Task DeleteAsync(Guid id);
    Task<DeviceDto> AssignToUserAsync(Guid deviceId, Guid userId);
    Task<DeviceDto> UnassignFromUserAsync(Guid deviceId, Guid userId);
    Task<IEnumerable<DeviceSearchResultDto>> SearchAsync(string query);
}