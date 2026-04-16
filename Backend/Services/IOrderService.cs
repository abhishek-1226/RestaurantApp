using RestaurantApp.DTOs;
using RestaurantApp.Models;
namespace RestaurantApp.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(int userId, OrderCreateDto dto);
        Task<List<OrderResponseDto>>GetOrdersByUserAsync(int userId);
        Task<List<OrderResponseDto>>GetOrderByRestaurantAsync(int restaurantId);
        Task<OrderResponseDto>GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<bool> UpdatePaymentStatusAsync(int orderId, string newPaymentStatus);
        Task<bool> SetPaymentMethodAsync(int orderId, string paymentMethod);
        Task<bool> CancelOrderAsync(int orderId,int UserId);
    }
}