using RestaurantApp.Models;
using RestaurantApp.DTOs;
namespace RestaurantApp.Services
{
    public interface IUserService
    {
        //User Registration
        Task<UserModel> RegisterUser(RegisterDto dto); 
        //User Validation
        Task<UserModel?>ValidateUser(LoginDto dto);
        //Get User by Id
        Task<UserResponseDto?>GetUserById(int id);
        //Get User by Email
        Task<UserModel?>GetUserByEmail(string email);
        //Get All Users
        Task<List<UserResponseDto>>GetAllUsers();
        //Update User
        //Delete User
        //Role Management
        Task<bool>AssignRole(int userId,int roleId);
        Task<bool>UpdateUserStatus(int userId,string status);
    }
   

}