using Microsoft.EntityFrameworkCore;
using RestaurantApp.Models;
namespace RestaurantApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            /*
            Database configuration typically. In that configuration, you specify:
            Database provider
            Connection string
            Logging options
            Lazy loading
            */
        }
        public DbSet<UserModel>Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<RestaurantModel> Restaurants { get; set; }
        public DbSet<MenuItemModel> MenuItems { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderItemModel> OrderItems { get; set; }
        public DbSet<OtpVerificationModel> OtpVerifications { get; set; }


        //Setting up relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // User-Role relationship - Many to One
            modelBuilder.Entity<UserModel>()
            .HasOne(u => u.Role)//Each user has one role
            .WithMany(r=>r.Users) //Each role can have many users
            .HasForeignKey(u => u.RoleId) //Foreign key in UserModel
            .OnDelete(DeleteBehavior.Restrict); //Don't delete
            //MenuItem-Restaurant relationship - Many to One
            modelBuilder.Entity<MenuItemModel>()
            .HasOne(m=>m.Restaurant) //Each menu item belongs to one restaurant
            .WithMany(r=>r.MenuItems)
            .HasForeignKey(m => m.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);//If restaurant is deleted, delete menu items
            //Order-User relationship - Many to One
            modelBuilder.Entity<OrderModel>()
            .HasOne(o=> o.User)
            .WithMany() //No navigation property in UserModel for orders
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            //Order-Restaurant relationship - Many to One
            modelBuilder.Entity<OrderModel>()
            .HasOne(o => o.Restaurant)
            .WithMany()
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
            //OrderItem-Order relationship - Many to One
            modelBuilder.Entity<OrderItemModel>()
            .HasOne(oi => oi.Order)
            .WithMany(o=>o.OrderItems)
            .HasForeignKey(oi=>oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            //OrderItem-MenuItem relationship - Many to One
            modelBuilder.Entity<OrderItemModel>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi=>oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleModel>().HasData(
                new RoleModel { Id = 1, Name = "Admin" },
                new RoleModel { Id = 2, Name = "Manager" },
                new RoleModel { Id = 3, Name = "Operator" },
                new RoleModel { Id = 4, Name = "Customer" }
            );
        }
        
    }
}