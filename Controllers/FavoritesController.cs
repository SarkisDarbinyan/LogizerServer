// FavoritesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogizerServer.Data;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;

namespace LogizerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Добавить уровень в избранное
        [HttpPost]
        public async Task<ActionResult> AddToFavorites(AddFavoriteDto addFavoriteDto)
        {
            // Проверяем существование пользователя и уровня
            var userExists = await _context.Users.AnyAsync(u => u.Id == addFavoriteDto.UserId);
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == addFavoriteDto.LevelId);

            if (!userExists || !levelExists)
            {
                return NotFound(new { error = "Пользователь или уровень не найден" });
            }

            // Проверяем, не добавлен ли уже уровень в избранное
            var existingFavorite = await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.UserId == addFavoriteDto.UserId && uf.LevelId == addFavoriteDto.LevelId);

            if (existingFavorite != null)
            {
                return BadRequest(new { error = "Уровень уже в избранном" });
            }

            var favorite = new UserFavorite
            {
                UserId = addFavoriteDto.UserId,
                LevelId = addFavoriteDto.LevelId
            };

            _context.UserFavorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Уровень добавлен в избранное", favoriteId = favorite.Id });
        }

        // Получить избранные уровни пользователя
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteLevelDto>>> GetUserFavorites(int userId)
        {
            var favorites = await _context.UserFavorites
                .Where(uf => uf.UserId == userId)
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

        // Удалить уровень из избранного
        [HttpDelete("{favoriteId}")]
        public async Task<ActionResult> RemoveFromFavorites(int favoriteId)
        {
            var favorite = await _context.UserFavorites.FindAsync(favoriteId);

            if (favorite == null)
            {
                return NotFound(new { error = "Запись избранного не найдена" });
            }

            _context.UserFavorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Уровень удален из избранного" });
        }

        // Удалить уровень из избранного по userId и levelId
        [HttpDelete("user/{userId}/level/{levelId}")]
        public async Task<ActionResult> RemoveFromFavoritesByUserAndLevel(int userId, int levelId)
        {
            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.LevelId == levelId);

            if (favorite == null)
            {
                return NotFound(new { error = "Уровень не найден в избранном" });
            }

            _context.UserFavorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Уровень удален из избранного" });
        }

        // Проверить, находится ли уровень в избранном у пользователя
        [HttpGet("user/{userId}/level/{levelId}")]
        public async Task<ActionResult<bool>> IsLevelInFavorites(int userId, int levelId)
        {
            var isFavorite = await _context.UserFavorites
                .AnyAsync(uf => uf.UserId == userId && uf.LevelId == levelId);

            return Ok(isFavorite);
        }
    }
}