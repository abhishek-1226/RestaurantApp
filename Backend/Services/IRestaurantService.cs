using RestaurantApp.Models;
namespace RestaurantApp.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantModel> CreateRestaurant(RestaurantModel restaurant);
        Task<List<RestaurantModel>> GetAllRestaurants();
        Task<RestaurantModel?> GetRestaurantById(int id);
        Task<RestaurantModel> UpdateRestaurant(int restaurantId, RestaurantModel updatedRestaurant);
        Task<bool> DeleteRestaurant(int restaurantId);
        Task<bool> AssignManager(int restaurantId, int userId);
        Task<bool>SetRestaurantStatus(int restaurantId, bool isActive);
        Task<List<RestaurantModel>>GetRestaurantbyCity(string city);
        Task<List<RestaurantModel>>GetRestaurantbyState(string state);

    }
}