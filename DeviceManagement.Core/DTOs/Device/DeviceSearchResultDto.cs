namespace DeviceManagement.Core.DTOs.Device;

public class DeviceSearchResultDto
{
    public DeviceDto Device { get; set; } = null!;
    public int Score { get; set; }
}