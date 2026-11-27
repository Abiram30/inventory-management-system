using Inventory.Api.DTO;

namespace Inventory.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task<object?> AuthenticateAsync(LoginRequestDto request);
    }
}