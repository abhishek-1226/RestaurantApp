using RestaurantApp.Models;
namespace RestaurantApp.Services
{
    public interface IMenuService
    {
        Task<MenuItemModel> AddMenuItem(MenuItemModel menuItem);
        Task<List<MenuItemModel>> GetMenuByRestaurant(int restaurantId);
        Task<MenuItemModel?> GetMenuItemById(int id);
        Task<MenuItemModel> UpdateMenuItem(int menuItemId, MenuItemModel updatedMenuItem);
        Task<bool> DeleteMenuItem(int menuItemId);
        Task<bool> SetAvailability(int menuItemId, bool isAvailable);
        Task<List<MenuItemModel>> GetmenubyCategory(int restaurantId, string category);
        Task<bool> UpdatePrice(int menuItemId, decimal newPrice);
        Task<bool> ApplyDiscount(int menuItemId, decimal discountPercentage);
    }
}