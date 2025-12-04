// AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogizerServer.Data;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;
using LogizerServer.Services;

namespace LogizerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public AuthController(
            ApplicationDbContext context,
            IJwtService jwtService,
            IPasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(registerDto.Username))
                return BadRequest(new { error = "Имя пользователя обязательно" });

            if (string.IsNullOrWhiteSpace(registerDto.Password))
                return BadRequest(new { error = "Пароль обязателен" });

            if (registerDto.Username.Length < 3)
                return BadRequest(new { error = "Имя пользователя должно быть не менее 3 символов" });

            if (registerDto.Password.Length < 6)
                return BadRequest(new { error = "Пароль должен быть не менее 6 символов" });

            // Проверяем, существует ли пользователь с таким именем
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == registerDto.Username.ToLower());

            if (existingUser != null)
            {
                return BadRequest(new { error = "Пользователь с таким именем уже существует" });
            }

            // Хэшируем пароль
            var passwordHash = _passwordService.HashPassword(registerDto.Password);

            // Создаем нового пользователя
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt
                }
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest(new { error = "Имя пользователя и пароль обязательны" });

            // Ищем пользователя по имени (без учета регистра)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == loginDto.Username.ToLower());

            if (user == null)
            {
                return Unauthorized(new { error = "Неверное имя пользователя или пароль" });
            }

            // Проверяем пароль
            if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { error = "Неверное имя пользователя или пароль" });
            }

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt
                }
            };

            return Ok(response);
        }
    }
}