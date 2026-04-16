using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.DTOs;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Tests
{
    public class OrderServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _orderService = new OrderService(_context);

            SeedData();
        }

        private void SeedData()
        {
            var role = new RoleModel { Id = 4, Name = "Customer" };
            _context.Roles.Add(role);

            var user = new UserModel
            {
                Id = 1,
                Name = "Test User",
                Email = "test@test.com",
                PasswordHash = "hash",
                RoleId = 4,
                Status = "Active"
            };
            _context.Users.Add(user);

            var restaurant = new RestaurantModel
            {
                Id = 1,
                Name = "Test Restaurant",
                Address = "123 Test St",
                City = "TestCity",
                State = "TestState",
                PinCode = 123456,
                PhoneNumber = "1234567890"
            };
            _context.Restaurants.Add(restaurant);

            var menuItem1 = new MenuItemModel
            {
                Id = 1,
                Name = "Burger",
                Price = 200.00m,
                RestaurantId = 1,
                IsAvailable = true
            };
            var menuItem2 = new MenuItemModel
            {
                Id = 2,
                Name = "Pizza",
                Price = 350.00m,
                RestaurantId = 1,
                IsAvailable = true
            };
            _context.MenuItems.AddRange(menuItem1, menuItem2);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_WithCorrectTotals()
        {
            var dto = new OrderCreateDto
            {
                RestaurantId = 1,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                PaymentMethod = "Cash on Delivery",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 1, Quantity = 2 },
                    new OrderItemDto { MenuItemId = 2, Quantity = 1 }
                }
            };

            var result = await _orderService.CreateOrderAsync(1, dto);

            Assert.NotNull(result);
            Assert.Equal(750.00m, result.Subtotal);    // 200*2 + 350*1
            Assert.Equal(37.50m, result.TaxAmount);     // 5% GST
            Assert.Equal(787.50m, result.TotalAmount);  // Subtotal + GST
            Assert.Equal("Pending", result.OrderStatus);
            Assert.Equal(2, result.OrderItems.Count);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenRestaurantNotFound()
        {
            var dto = new OrderCreateDto
            {
                RestaurantId = 999,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 1, Quantity = 1 }
                }
            };

            await Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrderAsync(1, dto));
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenMenuItemNotFound()
        {
            var dto = new OrderCreateDto
            {
                RestaurantId = 1,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 999, Quantity = 1 }
                }
            };

            await Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrderAsync(1, dto));
        }

        [Fact]
        public async Task GetOrdersByUserAsync_ShouldReturnUserOrders()
        {
            // Create an order first
            var dto = new OrderCreateDto
            {
                RestaurantId = 1,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 1, Quantity = 1 }
                }
            };
            await _orderService.CreateOrderAsync(1, dto);

            var orders = await _orderService.GetOrdersByUserAsync(1);

            Assert.Single(orders);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateStatus()
        {
            var dto = new OrderCreateDto
            {
                RestaurantId = 1,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 1, Quantity = 1 }
                }
            };
            var order = await _orderService.CreateOrderAsync(1, dto);

            var result = await _orderService.UpdateOrderStatusAsync(order.Id, "Confirmed");

            Assert.True(result);
            var updated = await _orderService.GetOrderByIdAsync(order.Id);
            Assert.Equal("Confirmed", updated.OrderStatus);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldCancel_WhenPending()
        {
            var dto = new OrderCreateDto
            {
                RestaurantId = 1,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 1, Quantity = 1 }
                }
            };
            var order = await _orderService.CreateOrderAsync(1, dto);

            var result = await _orderService.CancelOrderAsync(order.Id, 1);

            Assert.True(result);
            var cancelled = await _orderService.GetOrderByIdAsync(order.Id);
            Assert.Equal("Cancelled", cancelled.OrderStatus);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldThrow_WhenAlreadyDelivered()
        {
            var dto = new OrderCreateDto
            {
                RestaurantId = 1,
                DeliveryAddress = "456 Test Ave",
                ContactNumber = "9876543210",
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { MenuItemId = 1, Quantity = 1 }
                }
            };
            var order = await _orderService.CreateOrderAsync(1, dto);
            await _orderService.UpdateOrderStatusAsync(order.Id, "Delivered");

            await Assert.ThrowsAsync<Exception>(() => _orderService.CancelOrderAsync(order.Id, 1));
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldThrow_WhenNotFound()
        {
            await Assert.ThrowsAsync<Exception>(() => _orderService.GetOrderByIdAsync(999));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
