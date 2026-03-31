using DeviceManagement.Infrastructure.Entities;

namespace DeviceManagement.Infrastructure.Repositories.Interfaces;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string name, string manufacturer);
    Task<Device> CreateAsync(Device device);
    Task<Device> UpdateAsync(Device device);
    Task DeleteAsync(Device device);
}