using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;

        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MenuItemModel> AddMenuItem(MenuItemModel menuItem)
        {
            var restaurant = await _context.Restaurants.FindAsync(menuItem.RestaurantId);
            if (restaurant == null)
            {
                throw new Exception("Restaurant not found.");
            }

            menuItem.CreatedAt = DateTime.UtcNow;
            menuItem.IsAvailable = true;
            menuItem.IsDeleted = false;

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return menuItem;
        }

        public async Task<List<MenuItemModel>> GetMenuByRestaurant(int restaurantId)
        {
            return await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<MenuItemModel?> GetMenuItemById(int id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<MenuItemModel> UpdateMenuItem(int menuItemId, MenuItemModel updatedMenuItem)
        {
            var existing = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId && !m.IsDeleted);
            if (existing == null)
            {
                throw new Exception("Menu item not found.");
            }

            existing.Name = updatedMenuItem.Name;
            existing.Price = updatedMenuItem.Price;
            existing.Description = updatedMenuItem.Description;
            existing.IsAvailable = updatedMenuItem.IsAvailable;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteMenuItem(int menuItemId)
        {
            var item = await _context.MenuItems.FindAsync(menuItemId);
            if (item == null || item.IsDeleted) return false;

            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAvailability(int menuItemId, bool isAvailable)
        {
            var item = await _context.MenuItems.FindAsync(menuItemId);
            if (item == null || item.IsDeleted) return false;

            item.IsAvailable = isAvailable;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MenuItemModel>> GetmenubyCategory(int restaurantId, string category)
        {
            // Since MenuItemModel doesn't have a Category field, filter by name pattern
            return await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId && !m.IsDeleted
                    && m.Name.ToLower().Contains(category.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> UpdatePrice(int menuItemId, decimal newPrice)
        {
            var item = await _context.MenuItems.FindAsync(menuItemId);
            if (item == null || item.IsDeleted) return false;

            item.Price = newPrice;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApplyDiscount(int menuItemId, decimal discountPercentage)
        {
            var item = await _context.MenuItems.FindAsync(menuItemId);
            if (item == null || item.IsDeleted) return false;

            if (discountPercentage < 0 || discountPercentage > 100)
            {
                throw new Exception("Discount percentage must be between 0 and 100.");
            }

            item.Price = item.Price * (1 - discountPercentage / 100);
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
