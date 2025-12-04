// FavoritesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Добавить эту директиву
using LogizerServer.Data;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;

namespace LogizerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Добавить этот атрибут
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить ID текущего пользователя из токена
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        // Добавить уровень в избранное (обновленный)
        [HttpPost]
        public async Task<ActionResult> AddToFavorites(AddFavoriteDto addFavoriteDto)
        {
            var currentUserId = GetCurrentUserId();

            // Используем ID текущего пользователя из токена
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == addFavoriteDto.LevelId);

            if (!userExists || !levelExists)
            {
                return NotFound(new { error = "Пользователь или уровень не найден" });
            }

            // Проверяем, не добавлен ли уже уровень в избранное
            var existingFavorite = await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.UserId == currentUserId && uf.LevelId == addFavoriteDto.LevelId);

            if (existingFavorite != null)
            {
                return BadRequest(new { error = "Уровень уже в избранном" });
            }

            var favorite = new UserFavorite
            {
                UserId = currentUserId,
                LevelId = addFavoriteDto.LevelId
            };

            _context.UserFavorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Уровень добавлен в избранное", favoriteId = favorite.Id });
        }

        // Получить избранные уровни текущего пользователя (обновленный)
        [HttpGet("my-favorites")]
        public async Task<ActionResult<IEnumerable<FavoriteLevelDto>>> GetMyFavorites()
        {
            var currentUserId = GetCurrentUserId();

            var favorites = await _context.UserFavorites
                .Where(uf => uf.UserId == currentUserId)
                .Include(uf => uf.Level)
                .Select(uf => new FavoriteLevelDto
                {
                    FavoriteId = uf.Id,
                    LevelId = uf.Level.Id,
                    Name = uf.Level.Name,
                    Description = uf.Level.Description,
                    Difficulty = uf.Level.Difficulty,
                    LikeCount = uf.Level.LikeCount,
                    PlayCount = uf.Level.PlayCount,
                    AddedAt = uf.AddedAt
                })
                .ToListAsync();

            return Ok(favorites);
        }

        // ... остальные методы с аналогичными изменениями
    }
}