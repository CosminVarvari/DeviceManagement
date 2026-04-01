namespace DeviceManagement.Tests;

public static class TestConstants
{
    public static readonly Guid UserId = Guid.Parse("A2000000-0000-0000-0000-000000000002");
    public static readonly Guid AdminId = Guid.Parse("A1000000-0000-0000-0000-000000000001");
    public static readonly Guid DeviceId = Guid.Parse("B1000000-0000-0000-0000-000000000001");
    public static readonly Guid AssignedDeviceId = Guid.Parse("B2000000-0000-0000-0000-000000000002");
    public static readonly Guid NonExistentId = Guid.Parse("FF000000-0000-0000-0000-000000000000");
}