using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogizerServer.Data;
using LogizerServer.Models;
using LogizerServer.Models.DTOs;

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
        public async Task<ActionResult<CreateLevelDto>> CreateLevel(CreateLevelDto createLevelDto)
        {
            var existingLevel = await _context.Levels
                .FirstOrDefaultAsync(l => l.Name == createLevelDto.Name);

            if (existingLevel != null)
            {
                return BadRequest(new { error = "Уровень с таким названием уже существует" });
            }

            var level = new Level
            {
                Name = createLevelDto.Name,
                Description = createLevelDto.Description,
                LevelData = createLevelDto.LevelData,
            };

            _context.Levels.Add(level);
            await _context.SaveChangesAsync();
            
            return Ok(level);
        }
    }
}