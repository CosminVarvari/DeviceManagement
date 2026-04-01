using DeviceManagement.Core.DTOs.User;

namespace DeviceManagement.Core.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
}