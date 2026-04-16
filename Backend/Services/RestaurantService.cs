using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly AppDbContext _context;

        public RestaurantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantModel> CreateRestaurant(RestaurantModel restaurant)
        {
            restaurant.CreatedAt = DateTime.UtcNow;
            restaurant.IsActive = true;
            restaurant.IsDeleted = false;

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        public async Task<List<RestaurantModel>> GetAllRestaurants()
        {
            return await _context.Restaurants
                .Where(r => !r.IsDeleted)
                .Include(r => r.MenuItems.Where(m => !m.IsDeleted))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<RestaurantModel?> GetRestaurantById(int id)
        {
            return await _context.Restaurants
                .Include(r => r.MenuItems.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<RestaurantModel> UpdateRestaurant(int restaurantId, RestaurantModel updatedRestaurant)
        {
            var existing = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId && !r.IsDeleted);
            if (existing == null)
            {
                throw new Exception("Restaurant not found.");
            }

            existing.Name = updatedRestaurant.Name;
            existing.Address = updatedRestaurant.Address;
            existing.City = updatedRestaurant.City;
            existing.State = updatedRestaurant.State;
            existing.PinCode = updatedRestaurant.PinCode;
            existing.PhoneNumber = updatedRestaurant.PhoneNumber;
            existing.Description = updatedRestaurant.Description;
            existing.Email = updatedRestaurant.Email;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteRestaurant(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null || restaurant.IsDeleted) return false;

            restaurant.IsDeleted = true;
            restaurant.DeletedAt = DateTime.UtcNow;
            restaurant.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignManager(int restaurantId, int userId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null || restaurant.IsDeleted) return false;

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            restaurant.ManagerId = userId;
            restaurant.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetRestaurantStatus(int restaurantId, bool isActive)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null || restaurant.IsDeleted) return false;

            restaurant.IsActive = isActive;
            restaurant.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RestaurantModel>> GetRestaurantbyCity(string city)
        {
            return await _context.Restaurants
                .Where(r => !r.IsDeleted && r.City.ToLower() == city.ToLower())
                .ToListAsync();
        }

        public async Task<List<RestaurantModel>> GetRestaurantbyState(string state)
        {
            return await _context.Restaurants
                .Where(r => !r.IsDeleted && r.State.ToLower() == state.ToLower())
                .ToListAsync();
        }
    }
}
