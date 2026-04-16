using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.DTOs;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetMenuByRestaurant(int restaurantId)
        {
            var items = await _menuService.GetMenuByRestaurant(restaurantId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var item = await _menuService.GetMenuItemById(id);
            if (item == null)
            {
                return NotFound(new { message = "Menu item not found." });
            }
            return Ok(item);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> AddMenuItem([FromBody] MenuItemCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var menuItem = new MenuItemModel
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    RestaurantId = dto.RestaurantId
                };

                var created = await _menuService.AddMenuItem(menuItem);
                return CreatedAtAction(nameof(GetMenuItem), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItemUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var menuItem = new MenuItemModel
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    IsAvailable = dto.IsAvailable
                };

                var updated = await _menuService.UpdateMenuItem(id, menuItem);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var result = await _menuService.DeleteMenuItem(id);
            if (!result)
            {
                return NotFound(new { message = "Menu item not found." });
            }
            return Ok(new { message = "Menu item deleted successfully." });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> SetAvailability(int id, [FromBody] AvailabilityDto dto)
        {
            var result = await _menuService.SetAvailability(id, dto.IsAvailable);
            if (!result)
            {
                return NotFound(new { message = "Menu item not found." });
            }
            return Ok(new { message = $"Availability set to {dto.IsAvailable}." });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] PriceUpdateDto dto)
        {
            var result = await _menuService.UpdatePrice(id, dto.NewPrice);
            if (!result)
            {
                return NotFound(new { message = "Menu item not found." });
            }
            return Ok(new { message = "Price updated successfully." });
        }

        [HttpGet("restaurant/{restaurantId}/search")]
        public async Task<IActionResult> SearchMenu(int restaurantId, [FromQuery] string query)
        {
            var items = await _menuService.GetmenubyCategory(restaurantId, query ?? "");
            return Ok(items);
        }
    }

    public class AvailabilityDto
    {
        public bool IsAvailable { get; set; }
    }

    public class PriceUpdateDto
    {
        public decimal NewPrice { get; set; }
    }
}
