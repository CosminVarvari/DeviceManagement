namespace DeviceManagement.Infrastructure.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string Location { get; set; } = string.Empty;

    public ICollection<Device> AssignedDevices { get; set; } = new List<Device>();
}