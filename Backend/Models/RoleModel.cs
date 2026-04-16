using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace RestaurantApp.Models
{
    // This class represents the "Roles" table in the database
[Index(nameof(Name), IsUnique = true)] // Ensure role names are unique
public class RoleModel
    {
        // Primary Key of the Role table
        // EF Core automatically treats "Id" as the primary key
        public int Id { get; set; }

        // Name of the role (e.g., Admin, Manager, Operator, Customer)
        // This defines what type of user this role represents
        	[Required]
	[MaxLength(50)]
	public string Name { get; set; }

        // Navigation property representing relationship with Users
        // One Role can have multiple Users assigned to it
        public List<UserModel> Users { get; set; }
    }
}
