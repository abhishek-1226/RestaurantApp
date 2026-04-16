using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Tests
{
    public class RestaurantServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly RestaurantService _restaurantService;

        public RestaurantServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _restaurantService = new RestaurantService(_context);
        }

        [Fact]
        public async Task CreateRestaurant_ShouldCreateAndReturn()
        {
            var restaurant = new RestaurantModel
            {
                Name = "Test Restaurant",
                Address = "123 Test St",
                City = "Melbourne",
                State = "VIC",
                PinCode = 3000,
                PhoneNumber = "0400000000"
            };

            var result = await _restaurantService.CreateRestaurant(restaurant);

            Assert.NotNull(result);
            Assert.Equal("Test Restaurant", result.Name);
            Assert.True(result.IsActive);
            Assert.False(result.IsDeleted);
        }

        [Fact]
        public async Task GetAllRestaurants_ShouldReturnNonDeleted()
        {
            await _restaurantService.CreateRestaurant(new RestaurantModel
            {
                Name = "Active Restaurant",
                Address = "1 St",
                City = "Melbourne",
                State = "VIC",
                PinCode = 3000,
                PhoneNumber = "0400000001"
            });

            var deletedRest = await _restaurantService.CreateRestaurant(new RestaurantModel
            {
                Name = "Deleted Restaurant",
                Address = "2 St",
                City = "Sydney",
                State = "NSW",
                PinCode = 2000,
                PhoneNumber = "0400000002"
            });
            await _restaurantService.DeleteRestaurant(deletedRest.Id);

            var restaurants = await _restaurantService.GetAllRestaurants();

            Assert.Single(restaurants);
            Assert.Equal("Active Restaurant", restaurants[0].Name);
        }

        [Fact]
        public async Task GetRestaurantbyCity_ShouldFilterByCity()
        {
            await _restaurantService.CreateRestaurant(new RestaurantModel
            {
                Name = "Melbourne Cafe",
                Address = "1 St",
                City = "Melbourne",
                State = "VIC",
                PinCode = 3000,
                PhoneNumber = "0400000001"
            });
            await _restaurantService.CreateRestaurant(new RestaurantModel
            {
                Name = "Sydney Cafe",
                Address = "2 St",
                City = "Sydney",
                State = "NSW",
                PinCode = 2000,
                PhoneNumber = "0400000002"
            });

            var melbourneRestaurants = await _restaurantService.GetRestaurantbyCity("Melbourne");

            Assert.Single(melbourneRestaurants);
            Assert.Equal("Melbourne Cafe", melbourneRestaurants[0].Name);
        }

        [Fact]
        public async Task DeleteRestaurant_ShouldSoftDelete()
        {
            var restaurant = await _restaurantService.CreateRestaurant(new RestaurantModel
            {
                Name = "To Delete",
                Address = "1 St",
                City = "Melbourne",
                State = "VIC",
                PinCode = 3000,
                PhoneNumber = "0400000001"
            });

            var result = await _restaurantService.DeleteRestaurant(restaurant.Id);

            Assert.True(result);
            var deleted = await _context.Restaurants.FindAsync(restaurant.Id);
            Assert.True(deleted!.IsDeleted);
            Assert.False(deleted.IsActive);
        }

        [Fact]
        public async Task SetRestaurantStatus_ShouldToggleActive()
        {
            var restaurant = await _restaurantService.CreateRestaurant(new RestaurantModel
            {
                Name = "Toggle Restaurant",
                Address = "1 St",
                City = "Melbourne",
                State = "VIC",
                PinCode = 3000,
                PhoneNumber = "0400000001"
            });

            await _restaurantService.SetRestaurantStatus(restaurant.Id, false);
            var updated = await _restaurantService.GetRestaurantById(restaurant.Id);

            Assert.NotNull(updated);
            Assert.False(updated!.IsActive);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
