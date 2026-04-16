using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantApp.Data;
using RestaurantApp.DTOs;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Tests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "TestSuperSecretKeyForJWTWhichShouldBeLongEnoughAtLeast32Bytes!!" },
                    { "Jwt:Issuer", "TestApp" },
                    { "Jwt:Audience", "TestUsers" },
                    { "Jwt:ExpireMinutes", "60" }
                })
                .Build();

            _authService = new AuthService(config);
        }

        [Fact]
        public async Task HashPassword_ShouldReturnHashedString()
        {
            var password = "TestPassword123";
            var hash = await _authService.HashPassword(password);

            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.Contains(":", hash); // Should contain salt:hash format
        }

        [Fact]
        public async Task HashPassword_ShouldProduceDifferentHashesForSamePassword()
        {
            var password = "TestPassword123";
            var hash1 = await _authService.HashPassword(password);
            var hash2 = await _authService.HashPassword(password);

            Assert.NotEqual(hash1, hash2); // Different salts should produce different hashes
        }

        [Fact]
        public async Task VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
        {
            var password = "TestPassword123";
            var hash = await _authService.HashPassword(password);

            var result = await _authService.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public async Task VerifyPassword_ShouldReturnFalse_ForIncorrectPassword()
        {
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword456";
            var hash = await _authService.HashPassword(password);

            var result = await _authService.VerifyPassword(wrongPassword, hash);

            Assert.False(result);
        }

        [Fact]
        public async Task VerifyPassword_ShouldReturnFalse_ForInvalidHashFormat()
        {
            var result = await _authService.VerifyPassword("password", "invalidhash");

            Assert.False(result);
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldReturnValidToken()
        {
            var user = new UserModel
            {
                Id = 1,
                Name = "Test User",
                Email = "test@test.com",
                PasswordHash = "hash",
                Status = "Active",
                Role = new RoleModel { Id = 1, Name = "Admin" }
            };

            var token = await _authService.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
            // JWT tokens have 3 parts separated by dots
            Assert.Equal(3, token.Split('.').Length);
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldReturnDifferentTokens_ForDifferentUsers()
        {
            var user1 = new UserModel
            {
                Id = 1,
                Name = "User 1",
                Email = "user1@test.com",
                PasswordHash = "hash",
                Status = "Active",
                Role = new RoleModel { Id = 1, Name = "Admin" }
            };

            var user2 = new UserModel
            {
                Id = 2,
                Name = "User 2",
                Email = "user2@test.com",
                PasswordHash = "hash",
                Status = "Active",
                Role = new RoleModel { Id = 4, Name = "Customer" }
            };

            var token1 = await _authService.GenerateJwtToken(user1);
            var token2 = await _authService.GenerateJwtToken(user2);

            Assert.NotEqual(token1, token2);
        }
    }
}
