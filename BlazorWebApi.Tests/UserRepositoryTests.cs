using CoreApi;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApi.Tests
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _repository;
        private readonly AppDbContext _context;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _context = new AppDbContext(options);
            _repository = new UserRepository(_context);

           
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
                        
            _context.Users.Add(new User
            {
                Id = 1,
                Username = "john_doe",
                Email = "john_doe@example.com",  
                Password = "password123"          
            });

            _context.SaveChanges();

        }

        [Fact]
        public async Task GetUserById_ReturnsUser_WhenUserExists()
        {
            var user = await _repository.GetUserById(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("john_doe", user.Username);
            Assert.Equal("john_doe@example.com", user.Email);
            Assert.Equal("password123", user.Password);  
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
        {            
            var user = await _repository.GetUserById(99);
                        
            Assert.Null(user);
        }

        [Fact]
        public async Task AddUser_AddsUserToDatabase()
        {
            
            var newUser = new User
            {
                Id = 2,
                Username = "jane_doe",
                Email = "jane_doe@example.com",  
                Password = "password123"          
            };
                        
            await _repository.AddUser(newUser);
            var addedUser = await _context.Users.FindAsync(2);

            
            Assert.NotNull(addedUser);
            Assert.Equal("jane_doe", addedUser.Username);
            Assert.Equal("jane_doe@example.com", addedUser.Email);
            Assert.Equal("password123", addedUser.Password); 
        }




    }
}
