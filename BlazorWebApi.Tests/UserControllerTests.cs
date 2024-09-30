using CoreApi;
using CoreApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace BlazorWebApi.Tests
{
    public class UserControllerTests
    {
        private readonly UsersController _controller;
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;

        public UserControllerTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            
            var jwtSettings = new JwtSettings
            {
                Key = "superSecretKey@345adfadfasdfadfasdfadfadfadfadfadfadfasdfasf",  //superSecretKey@345_super_secret_256bit_key
                Issuer = "https://localhost:7077",
                Audience = "https://localhost:7074"
            };
            
            _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
            _mockJwtSettings.Setup(s => s.Value).Returns(jwtSettings);
                        
            _controller = new UsersController(_mockRepository.Object, _mockJwtSettings.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddUser_ReturnsCreatedAtAction()
        {
            // Arrange
            var newUser = new User { Id = 1, Username = "john_doe" };
            _mockRepository.Setup(repo => repo.AddUser(newUser)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddUser(newUser);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetUser", createdResult.ActionName);
            Assert.Equal(newUser.Id, createdResult.RouteValues["id"]);
        }

    }
}