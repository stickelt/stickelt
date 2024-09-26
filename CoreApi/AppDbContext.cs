using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoreApi
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
                
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed dummy users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "john_doe", Password = "password123", Email = "john@example.com" },
                new User { Id = 2, Username = "jane_doe", Password = "password456", Email = "jane@example.com" }
            );
            // Specify the precision and scale for the decimal type
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); // 18 precision and 2 decimal places

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    var hasher = new PasswordHasher<User>();

        //    modelBuilder.Entity<User>().HasData(
        //        new User
        //        {
        //            Id = 1,
        //            Username = "john_doe",
        //            Password = hasher.HashPassword(null, "password123"),
        //            Email = "john@example.com"
        //        },
        //        new User
        //        {
        //            Id = 2,
        //            Username = "jane_doe",
        //            Password = hasher.HashPassword(null, "password456"),
        //            Email = "jane@example.com"
        //        }
        //    );

        //    base.OnModelCreating(modelBuilder);
        //}


    }
}
