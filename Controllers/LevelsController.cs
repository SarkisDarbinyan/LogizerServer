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

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateLevelDto>> GetLevel(int id)
        {
            var level = await _context.Levels.FindAsync(id);

            if (level == null)
                return NotFound();

            return Ok(LevelToDto(level));
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
            //return CreatedAtAction(nameof(GetLevel), new { id = level.Id }, LevelToDto(level));
        }

        private static CreateLevelDto LevelToDto(Level level) => new CreateLevelDto
        {
            Name = level.Name,
            Description = level.Description,
            LevelData = level.LevelData
        };
    }
}