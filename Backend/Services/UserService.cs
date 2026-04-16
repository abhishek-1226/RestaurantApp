using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.DTOs;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public UserService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<UserModel> RegisterUser(RegisterDto dto)
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }

            var hashedPassword = await _authService.HashPassword(dto.Password);

            var user = new UserModel
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId ?? 4, // Default to Customer role
                Status = "Active",
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Load the role navigation property
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return user;
        }

        public async Task<UserModel?> ValidateUser(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);

            if (user == null)
            {
                return null;
            }

            // Check if account is locked
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new Exception("Account is locked. Please try again later.");
            }

            // Check if account is active
            if (user.Status != "Active")
            {
                throw new Exception("Account is not active. Please contact support.");
            }

            var isPasswordValid = await _authService.VerifyPassword(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;
                }
                await _context.SaveChangesAsync();
                return null;
            }

            // Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<UserResponseDto?> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null) return null;

            return MapToDto(user);
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        public async Task<bool> AssignRole(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted) return false;

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserStatus(int userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted) return false;

            user.Status = status;
            user.UpdatedAt = DateTime.UtcNow;

            if (status == "Deleted")
            {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private UserResponseDto MapToDto(UserModel user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleName = user.Role?.Name ?? "Unknown",
                Status = user.Status,
                IsVerified = user.IsVerified
            };
        }
    }
}
