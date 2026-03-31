using DeviceManagement.Core.DTOs.Device;
using DeviceManagement.Infrastructure.Entities;
using DeviceManagement.Core.Services.Interfaces;
using DeviceManagement.Infrastructure.Repositories.Interfaces;

namespace DeviceManagement.Core.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepo;
    private readonly IUserRepository _userRepo;

    public DeviceService(IDeviceRepository deviceRepo, IUserRepository userRepo)
    {
        _deviceRepo = deviceRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        var devices = await _deviceRepo.GetAllAsync();
        return devices.Select(MapToDto);
    }

    public async Task<DeviceDto> GetByIdAsync(Guid id)
    {
        var device = await _deviceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Device with ID {id} not found.");
        return MapToDto(device);
    }

    public async Task<DeviceDto> CreateAsync(DeviceRequestDto dto)
    {
        var exists = await _deviceRepo.ExistsAsync(dto.Name, dto.Manufacturer);
        if (exists)
            throw new InvalidOperationException(
                $"A device '{dto.Name}' from '{dto.Manufacturer}' already exists.");

        var device = new Device
        {
            Name = dto.Name,
            Manufacturer = dto.Manufacturer,
            Type = dto.Type,
            OperatingSystem = dto.OperatingSystem,
            OsVersion = dto.OsVersion,
            Processor = dto.Processor,
            RamAmount = dto.RamAmount,
            Description = dto.Description
        };

        var created = await _deviceRepo.CreateAsync(device);
        return MapToDto(created);
    }

    public async Task<DeviceDto> UpdateAsync(Guid id, DeviceRequestDto dto)
    {
        var device = await _deviceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Device with ID {id} not found.");

        device.Name = dto.Name;
        device.Manufacturer = dto.Manufacturer;
        device.Type = dto.Type;
        device.OperatingSystem = dto.OperatingSystem;
        device.OsVersion = dto.OsVersion;
        device.Processor = dto.Processor;
        device.RamAmount = dto.RamAmount;
        device.Description = dto.Description;
        device.UpdatedAt = DateTime.UtcNow;

        var updated = await _deviceRepo.UpdateAsync(device);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var device = await _deviceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Device with ID {id} not found.");
        await _deviceRepo.DeleteAsync(device);
    }

    public async Task<DeviceDto> AssignToUserAsync(Guid deviceId, Guid userId)
    {
        var device = await _deviceRepo.GetByIdAsync(deviceId)
            ?? throw new KeyNotFoundException($"Device with ID {deviceId} not found.");

        if (device.AssignedUserId.HasValue)
            throw new InvalidOperationException("Device is already assigned to another user.");

        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        device.AssignedUserId = userId;
        device.UpdatedAt = DateTime.UtcNow;

        var updated = await _deviceRepo.UpdateAsync(device);
        return MapToDto(updated);
    }

    public async Task<DeviceDto> UnassignFromUserAsync(Guid deviceId, Guid userId)
    {
        var device = await _deviceRepo.GetByIdAsync(deviceId)
            ?? throw new KeyNotFoundException($"Device with ID {deviceId} not found.");

        if (device.AssignedUserId != userId)
            throw new InvalidOperationException("You can only unassign a device assigned to you.");

        device.AssignedUserId = null;
        device.UpdatedAt = DateTime.UtcNow;

        var updated = await _deviceRepo.UpdateAsync(device);
        return MapToDto(updated);
    }

    private static DeviceDto MapToDto(Device d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Manufacturer = d.Manufacturer,
        Type = d.Type,
        OperatingSystem = d.OperatingSystem,
        OsVersion = d.OsVersion,
        Processor = d.Processor,
        RamAmount = d.RamAmount,
        Description = d.Description,
        AssignedUserId = d.AssignedUserId,
        AssignedUserName = d.AssignedUser?.Name
    };
}