using Inventory.Api.Data;
using Inventory.Api.DTO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object?> AuthenticateAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

            if (user == null) return null;

            return new
            {
                message = "Login successful",
                userId = user.Id,
                username = user.Username,
                role = user.Role
            };
        }
    }
}