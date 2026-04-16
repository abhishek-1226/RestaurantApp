using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.DTOs;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRestaurants()
        {
            var restaurants = await _restaurantService.GetAllRestaurants();
            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound(new { message = "Restaurant not found." });
            }
            return Ok(restaurant);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restaurant = new RestaurantModel
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PinCode = dto.PinCode,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description,
                Email = dto.Email
            };

            var created = await _restaurantService.CreateRestaurant(restaurant);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var restaurant = new RestaurantModel
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State,
                    PinCode = dto.PinCode,
                    PhoneNumber = dto.PhoneNumber,
                    Description = dto.Description,
                    Email = dto.Email,
                    IsActive = dto.IsActive
                };

                var updated = await _restaurantService.UpdateRestaurant(id, restaurant);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var result = await _restaurantService.DeleteRestaurant(id);
            if (!result)
            {
                return NotFound(new { message = "Restaurant not found." });
            }
            return Ok(new { message = "Restaurant deleted successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/manager")]
        public async Task<IActionResult> AssignManager(int id, [FromBody] ManagerAssignDto dto)
        {
            var result = await _restaurantService.AssignManager(id, dto.ManagerId);
            if (!result)
            {
                return NotFound(new { message = "Restaurant or user not found." });
            }
            return Ok(new { message = "Manager assigned successfully." });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] RestaurantStatusDto dto)
        {
            var result = await _restaurantService.SetRestaurantStatus(id, dto.IsActive);
            if (!result)
            {
                return NotFound(new { message = "Restaurant not found." });
            }
            return Ok(new { message = $"Restaurant status set to {(dto.IsActive ? "active" : "inactive")}." });
        }

        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetByCity(string city)
        {
            var restaurants = await _restaurantService.GetRestaurantbyCity(city);
            return Ok(restaurants);
        }

        [HttpGet("state/{state}")]
        public async Task<IActionResult> GetByState(string state)
        {
            var restaurants = await _restaurantService.GetRestaurantbyState(state);
            return Ok(restaurants);
        }
    }

    public class ManagerAssignDto
    {
        public int ManagerId { get; set; }
    }

    public class RestaurantStatusDto
    {
        public bool IsActive { get; set; }
    }
}