using LogizerServer.Data;
using LogizerServer.Exceptions;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LogizerServer.Services
{
    public interface ILevelInteractionService
    {
        Task<LevelRatingResponseDto> RateLevel(int userId, RateLevelDto rateLevelDto);
        Task<LevelLikeResponseDto> Like(int userId, LikeLevelDto likeLevelDto);
        Task<LevelRatingResponseDto?> GetUserRating(int userId, int levelId);
        Task<LevelLikeResponseDto> GetUserLikeStatus(int userId, int levelId);
        Task<LevelRatingResponseDto> GetLevelAverageRating(int levelId);
    }

    public class LevelInteractionService : ILevelInteractionService
    {
        private readonly ApplicationDbContext _context;

        public LevelInteractionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LevelRatingResponseDto> RateLevel(int userId, RateLevelDto rateLevelDto)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == userId);
            var level = await _context.Levels.FindAsync(rateLevelDto.LevelId);

            if (!user || level == null) throw new NotFoundException("Пользователь или уровень не найден");

            if (rateLevelDto.DifficultyRating == Difficulties.Unknown) throw new ValidationException("Неверная оценка сложности");

            var existingRating = await _context.LevelRatings.FirstOrDefaultAsync(lr => lr.UserId == userId && lr.LevelId == rateLevelDto.LevelId);

            LevelRating rating;

            if (existingRating != null)
            {
                existingRating.DifficultyRating = rateLevelDto.DifficultyRating;
                existingRating.RatedAt = DateTime.UtcNow;
                rating = existingRating;
            }
            else
            {
                rating = new LevelRating
                {
                    UserId = userId,
                    LevelId = rateLevelDto.LevelId,
                    DifficultyRating = rateLevelDto.DifficultyRating
                };
                _context.LevelRatings.Add(rating);
            }

            await _context.SaveChangesAsync();
            await UpdateLevelAverageDifficulty(rateLevelDto.LevelId);

            var averageDifficulty = await CalculateAverageDifficulty(rateLevelDto.LevelId);
            var totalRatings = await _context.LevelRatings.CountAsync(lr => lr.LevelId == rateLevelDto.LevelId);

            return new LevelRatingResponseDto
            {
                AverageDifficulty = averageDifficulty,
                TotalRatings = totalRatings
            };
        }

        public async Task<LevelLikeResponseDto> Like(int userId, LikeLevelDto likeLevelDto)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == userId);
            var level = await _context.Levels.FindAsync(likeLevelDto.LevelId);

            if (!user || level == null) throw new NotFoundException("Пользователь или уровень не найден");
            
            var existingLike = await _context.LevelLikes.FirstOrDefaultAsync(ll => ll.UserId == userId && ll.LevelId == likeLevelDto.LevelId);

            if (existingLike != null) return new LevelLikeResponseDto{ TotalLikes = level.LikeCount };

            var like = new LevelLike
            {
                UserId = userId,
                LevelId = likeLevelDto.LevelId
            };

            _context.LevelLikes.Add(like);
            level.LikeCount++;

            await _context.SaveChangesAsync();
            return new LevelLikeResponseDto { TotalLikes = level.LikeCount };
        }

        public async Task<LevelRatingResponseDto?> GetUserRating(int userId, int levelId)
        {
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == levelId);
            if (!levelExists) return null;

            var rating = await _context.LevelRatings
                .Include(lr => lr.Level)
                .FirstOrDefaultAsync(lr => lr.UserId == userId && lr.LevelId == levelId);

            if (rating == null) return null;

            var averageDifficulty = await CalculateAverageDifficulty(levelId);
            var totalRatings = await _context.LevelRatings.CountAsync(lr => lr.LevelId == levelId);

            return new LevelRatingResponseDto
            {
                AverageDifficulty = averageDifficulty,
                TotalRatings = totalRatings
            };
        }

        public async Task<LevelLikeResponseDto> GetUserLikeStatus(int userId, int levelId)
        {
            var level = await _context.Levels.FindAsync(levelId) ?? throw new NotFoundException("Уровень не найден");
            
            var like = await _context.LevelLikes.Include(ll => ll.Level)
                .FirstOrDefaultAsync(ll => ll.UserId == userId && ll.LevelId == levelId);

            return new LevelLikeResponseDto { TotalLikes = level.LikeCount};
        }

        public async Task<LevelRatingResponseDto> GetLevelAverageRating(int levelId)
        {
            var levelExists = await _context.Levels.AnyAsync(l => l.Id == levelId);
            if (!levelExists)
            {
                throw new NotFoundException("Уровень не найден");
            }

            var averageDifficulty = await CalculateAverageDifficulty(levelId);
            var totalRatings = await _context.LevelRatings
                .CountAsync(lr => lr.LevelId == levelId);

            return new LevelRatingResponseDto
            {
                AverageDifficulty = averageDifficulty,
                TotalRatings = totalRatings
            };
        }

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