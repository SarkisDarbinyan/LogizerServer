using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogizerServer.Data;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LogizerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LevelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<GetLevelDto>>> GetLevel(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name parameter is required");

            var levels = await _context.Levels
                .Where(l => l.Name.ToLower().Contains(name.ToLower()))
                .Select(l => new GetLevelDto
                {
                    LevelId = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    LevelData = l.LevelData,
                    Difficulty = l.Difficulty,
                    LikeCount = l.LikeCount,
                    PlayCount = l.PlayCount
                })
                .ToListAsync();

            if (levels == null)
                return NotFound();

            return Ok(levels);
        }


        [HttpGet]
        public async Task<ActionResult<GetLevelDto>> GetLevelList()
        {
            var levelDtos = await _context.Levels
                .Select(level => new GetLevelDto
                {
                    LevelId = level.Id,
                    Name = level.Name,
                    Description = level.Description,
                    LevelData = level.LevelData,
                    Difficulty = level.Difficulty,
                    LikeCount = level.LikeCount,
                    PlayCount = level.PlayCount
                })
                .ToListAsync();

            if (levelDtos.Count == 0)
                return NotFound();

            return Ok(levelDtos);
        }


        [HttpPost]
        [Authorize] 
        public async Task<ActionResult<CreateLevelDto>> CreateLevel(CreateLevelDto createLevelDto)
        {
            int currentUserId = createLevelDto.UserId;

            var existingLevelWithDifferentUser = await _context.Levels
                .FirstOrDefaultAsync(l => l.Name == createLevelDto.Name && l.CreatorId != currentUserId);

            if (existingLevelWithDifferentUser != null)
            {
                return BadRequest(new { error = "Уровень с таким названием уже существует у другого пользователя" });
            }

            var existingLevelSameUser = await _context.Levels
                .FirstOrDefaultAsync(l => l.Name == createLevelDto.Name && l.CreatorId == currentUserId);

            if (existingLevelSameUser != null)
            {
                existingLevelSameUser.Description = createLevelDto.Description;
                existingLevelSameUser.LevelData = createLevelDto.LevelData;

                _context.Levels.Update(existingLevelSameUser);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Уровень успешно обновлен",
                    level = existingLevelSameUser,
                    isUpdated = true
                });
            }
            else
            {
                // Создаем новый уровень
                var level = new Level
                {
                    Name = createLevelDto.Name,
                    Description = createLevelDto.Description,
                    LevelData = createLevelDto.LevelData,
                    CreatorId = currentUserId
                };

                _context.Levels.Add(level);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Уровень успешно создан",
                    level = level,
                    isUpdated = false
                });
            }
        }
    }
}