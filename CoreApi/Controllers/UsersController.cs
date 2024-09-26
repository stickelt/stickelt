using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public UsersController(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            await _userRepository.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            await _userRepository.UpdateUser(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);
            return NoContent();
        }

        // Login method to authenticate users
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Check if the user exists in the database
            var user = await _userRepository.GetUserByUsername(model.Username);
            if (user == null || !VerifyPassword(model.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user.Username);
            return Ok(new { token });
        }

        // Generate JWT token
        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Simple password verification (or use hashing for production)
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }
    }
}
