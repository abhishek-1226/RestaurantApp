using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantApp.Data;
using RestaurantApp.DTOs;
using RestaurantApp.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(int userId, OrderCreateDto dto)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == dto.RestaurantId);
            if (restaurant == null)
            {
                throw new Exception("Restaurant not found");
            }

            var order = new OrderModel
            {
                UserId = userId,
                RestaurantId = dto.RestaurantId,
                DeliveryAddress = dto.DeliveryAddress,
                ContactNumber = dto.ContactNumber,
                Status = "Pending",
                PaymentStatus = "Pending",
                PaymentMethod = dto.PaymentMethod,
                OrderItems = new List<OrderItemModel>()
            };

            decimal subtotal = 0;
            if (dto.OrderItems != null)
            {
                foreach(var item in dto.OrderItems)
                {
                    var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
                    if(menuItem == null)
                    {
                        throw new Exception($"Menu item with id {item.MenuItemId} not found");
                    }
                    var unitprice = menuItem.Price;
                    var totalPrice = unitprice * item.Quantity;
                    subtotal = subtotal + totalPrice;

                    order.OrderItems.Add(new OrderItemModel
                    {
                        MenuItemId = item.MenuItemId,
                        MenuItemName = menuItem.Name,
                        Quantity = item.Quantity,
                        UnitPrice = menuItem.Price,
                        SpecialInstructions = item.SpecialInstructions
                    });
                }
            }

            order.TotalAmount = subtotal;
            order.GST = subtotal * 0.05m; 
            order.FinalBillAmount = order.TotalAmount + order.GST; 

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return MapToOrderResponseDto(order);
        }

        private OrderResponseDto MapToOrderResponseDto(OrderModel order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                RestaurantId = order.RestaurantId,
                DeliveryAddress = order.DeliveryAddress ?? string.Empty,
                DeliveryStatus = order.DeliveryStatus ?? string.Empty,
                ContactNumber = order.ContactNumber ?? string.Empty,
                OrderStatus = order.Status ?? string.Empty,
                PaymentStatus = order.PaymentStatus ?? string.Empty,
                PaymentMethod = order.PaymentMethod ?? string.Empty,
                Subtotal = order.TotalAmount,
                TaxAmount = order.GST,
                TotalAmount = order.FinalBillAmount,
                DeliveryFee = order.DeliveryFee,
                CouponCode = order.CouponCode,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemResponseDto
                {
                    MenuItemId = oi.MenuItemId,
                    ItemName = oi.MenuItemName ?? string.Empty,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.UnitPrice * oi.Quantity,  
                    UnitPrice = oi.UnitPrice,
                    SpecialInstructions = oi.SpecialInstructions
                }).ToList() ?? new List<OrderItemResponseDto>()
            };
        }

        public async Task<List<OrderResponseDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .ToListAsync();
            return orders.Select(MapToOrderResponseDto).ToList();
        }

        public async Task<List<OrderResponseDto>> GetOrderByRestaurantAsync(int restaurantId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.RestaurantId == restaurantId)
                .ToListAsync();
            return orders.Select(MapToOrderResponseDto).ToList();
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            return MapToOrderResponseDto(order);
        }       

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            order.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePaymentStatusAsync(int orderId, string newPaymentStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            order.PaymentStatus = newPaymentStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetPaymentMethodAsync(int orderId, string paymentMethod)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            order.PaymentMethod = paymentMethod;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if(order == null)
            {
                throw new Exception("Order not found or unauthorized");
            }
            
            if(order.Status == "Delivered" || order.Status == "Cancelled")
            {
                throw new Exception("Order cannot be cancelled");
            }

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}