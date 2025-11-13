using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogizerServer.Data;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;

namespace LogizerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevelInteractionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LevelInteractionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Оценить уровень по сложности
        [HttpPost("rate")]
        public async Task<ActionResult<LevelRatingResponseDto>> RateLevel(RateLevelDto rateLevelDto)
        {
            // Проверяем существование пользователя и уровня
            var userExists = await _context.Users.AnyAsync(u => u.Id == rateLevelDto.UserId);
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == rateLevelDto.LevelId);

            if (!userExists || !levelExists)
            {
                return NotFound(new { error = "Пользователь или уровень не найден" });
            }

            // Проверяем валидность оценки
            if (rateLevelDto.DifficultyRating == Difficulties.Unknown)
            {
                return BadRequest(new { error = "Неверная оценка сложности" });
            }

            // Ищем существующую оценку
            var existingRating = await _context.LevelRatings
                .FirstOrDefaultAsync(lr => lr.UserId == rateLevelDto.UserId && lr.LevelId == rateLevelDto.LevelId);

            LevelRating rating;
            var level = await _context.Levels.FindAsync(rateLevelDto.LevelId);

            if (existingRating != null)
            {
                // Обновляем существующую оценку
                existingRating.DifficultyRating = rateLevelDto.DifficultyRating;
                existingRating.RatedAt = DateTime.UtcNow;
                rating = existingRating;
            }
            else
            {
                // Создаем новую оценку
                rating = new LevelRating
                {
                    UserId = rateLevelDto.UserId,
                    LevelId = rateLevelDto.LevelId,
                    DifficultyRating = rateLevelDto.DifficultyRating
                };
                _context.LevelRatings.Add(rating);
            }

            await _context.SaveChangesAsync();

            // Пересчитываем среднюю сложность уровня
            await UpdateLevelAverageDifficulty(rateLevelDto.LevelId);

            // Получаем обновленные данные
            var averageDifficulty = await CalculateAverageDifficulty(rateLevelDto.LevelId);
            var totalRatings = await _context.LevelRatings
                .CountAsync(lr => lr.LevelId == rateLevelDto.LevelId);

            var response = new LevelRatingResponseDto
            {
                RatingId = rating.Id,
                LevelId = rating.LevelId,
                LevelName = level.Name,
                DifficultyRating = rating.DifficultyRating,
                RatedAt = rating.RatedAt,
                AverageDifficulty = averageDifficulty,
                TotalRatings = totalRatings
            };

            return Ok(response);
        }

        // Поставить/убрать лайк уровню
        [HttpPost("like")]
        public async Task<ActionResult<LevelLikeResponseDto>> ToggleLike(LikeLevelDto likeLevelDto)
        {
            // Проверяем существование пользователя и уровня
            var userExists = await _context.Users.AnyAsync(u => u.Id == likeLevelDto.UserId);
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == likeLevelDto.LevelId);

            if (!userExists || !levelExists)
            {
                return NotFound(new { error = "Пользователь или уровень не найден" });
            }

            var level = await _context.Levels.FindAsync(likeLevelDto.LevelId);

            // Ищем существующий лайк
            var existingLike = await _context.LevelLikes
                .FirstOrDefaultAsync(ll => ll.UserId == likeLevelDto.UserId && ll.LevelId == likeLevelDto.LevelId);

            LevelLikeResponseDto response;

            if (existingLike != null)
            {
                // Убираем лайк
                _context.LevelLikes.Remove(existingLike);
                level.LikeCount--;
                response = new LevelLikeResponseDto
                {
                    LikeId = existingLike.Id,
                    LevelId = level.Id,
                    LevelName = level.Name,
                    IsLiked = false,
                    TotalLikes = level.LikeCount,
                    LikedAt = existingLike.LikedAt
                };
            }
            else
            {
                // Ставим лайк
                var like = new LevelLike
                {
                    UserId = likeLevelDto.UserId,
                    LevelId = likeLevelDto.LevelId
                };
                _context.LevelLikes.Add(like);
                level.LikeCount++;

                await _context.SaveChangesAsync();

                response = new LevelLikeResponseDto
                {
                    LikeId = like.Id,
                    LevelId = level.Id,
                    LevelName = level.Name,
                    IsLiked = true,
                    TotalLikes = level.LikeCount,
                    LikedAt = like.LikedAt
                };
            }

            await _context.SaveChangesAsync();
            return Ok(response);
        }

        // Получить оценку пользователя для уровня
        [HttpGet("user/{userId}/level/{levelId}/rating")]
        public async Task<ActionResult<LevelRatingResponseDto>> GetUserRating(int userId, int levelId)
        {
            // Проверяем существование уровня
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == levelId);
            if (!levelExists)
            {
                return NotFound(new { error = "Уровень не найден" });
            }

            var rating = await _context.LevelRatings
                .Include(lr => lr.Level)
                .FirstOrDefaultAsync(lr => lr.UserId == userId && lr.LevelId == levelId);

            if (rating == null)
            {
                return NotFound(new { error = "Оценка не найдена" });
            }

            var averageDifficulty = await CalculateAverageDifficulty(levelId);
            var totalRatings = await _context.LevelRatings
                .CountAsync(lr => lr.LevelId == levelId);

            var response = new LevelRatingResponseDto
            {
                RatingId = rating.Id,
                LevelId = rating.LevelId,
                LevelName = rating.Level.Name,
                DifficultyRating = rating.DifficultyRating,
                RatedAt = rating.RatedAt,
                AverageDifficulty = averageDifficulty,
                TotalRatings = totalRatings
            };

            return Ok(response);
        }

        // Получить статус лайка пользователя для уровня
        [HttpGet("user/{userId}/level/{levelId}/like")]
        public async Task<ActionResult<LevelLikeResponseDto>> GetUserLikeStatus(int userId, int levelId)
        {
            // Проверяем существование уровня
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == levelId);
            if (!levelExists)
            {
                return NotFound(new { error = "Уровень не найден" });
            }

            var like = await _context.LevelLikes
                .Include(ll => ll.Level)
                .FirstOrDefaultAsync(ll => ll.UserId == userId && ll.LevelId == levelId);

            var level = await _context.Levels.FindAsync(levelId);

            var response = new LevelLikeResponseDto
            {
                LevelId = levelId,
                LevelName = level.Name,
                IsLiked = like != null,
                TotalLikes = level.LikeCount,
                LikedAt = like?.LikedAt ?? DateTime.MinValue
            };

            if (like != null)
            {
                response.LikeId = like.Id;
            }

            return Ok(response);
        }

        // Получить среднюю оценку сложности уровня
        [HttpGet("level/{levelId}/average-rating")]
        public async Task<ActionResult> GetLevelAverageRating(int levelId)
        {
            // Проверяем существование уровня
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == levelId);
            if (!levelExists)
            {
                return NotFound(new { error = "Уровень не найден" });
            }

            var averageDifficulty = await CalculateAverageDifficulty(levelId);
            var totalRatings = await _context.LevelRatings
                .CountAsync(lr => lr.LevelId == levelId);

            return Ok(new
            {
                LevelId = levelId,
                AverageDifficulty = averageDifficulty,
                TotalRatings = totalRatings
            });
        }

        // Вспомогательные методы
        private async Task<Difficulties> CalculateAverageDifficulty(int levelId)
        {
            var ratings = await _context.LevelRatings
                .Where(lr => lr.LevelId == levelId)
                .ToListAsync();

            if (!ratings.Any())
                return Difficulties.Unknown;

            var average = (int)Math.Round(ratings.Average(r => (int)r.DifficultyRating));
            return (Difficulties)average;
        }

        private async Task UpdateLevelAverageDifficulty(int levelId)
        {
            var level = await _context.Levels.FindAsync(levelId);
            if (level != null)
            {
                level.Difficulty = await CalculateAverageDifficulty(levelId);
                await _context.SaveChangesAsync();
            }
        }
    }
}