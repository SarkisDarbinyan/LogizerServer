using LogizerServer.Exceptions;
using LogizerServer.Models.DTOs;
using LogizerServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace LogizerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LevelInteractionsController : ControllerBase
    {
        private readonly ILevelInteractionService _levelInteractionService;

        public LevelInteractionsController(ILevelInteractionService levelInteractionService)
        {
            _levelInteractionService = levelInteractionService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Неверный токен авторизации");
            }
            return userId;
        }

        [HttpPost("rate")]
        public async Task<ActionResult<LevelRatingResponseDto>> RateLevel(RateLevelDto rateLevelDto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _levelInteractionService.RateLevel(userId, rateLevelDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("like")]
        public async Task<ActionResult<LevelLikeResponseDto>> ToggleLike(LikeLevelDto likeLevelDto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _levelInteractionService.Like(userId, likeLevelDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("level/{levelId}/rating")]
        public async Task<ActionResult<LevelRatingResponseDto>> GetUserRating(int levelId)
        {
            try
            {
                var userId = GetUserId();
                var rating = await _levelInteractionService.GetUserRating(userId, levelId);

                if (rating == null) return NotFound(new { error = "Оценка не найдена" });

                return Ok(rating);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("level/{levelId}/like")]
        public async Task<ActionResult<LevelLikeResponseDto>> GetUserLikeStatus(int levelId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _levelInteractionService.GetUserLikeStatus(userId, levelId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("level/{levelId}/average-rating")]
        [AllowAnonymous]
        public async Task<ActionResult> GetLevelAverageRating(int levelId)
        {
            try
            {
                var result = await _levelInteractionService.GetLevelAverageRating(levelId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }
    }
}