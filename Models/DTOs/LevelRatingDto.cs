// LevelRatingDto.cs
namespace LogizerServer.Models.DTOs
{
    public class RateLevelDto
    {
        public int LevelId { get; set; }
        public Difficulties DifficultyRating { get; set; }
    }

    public class LevelRatingResponseDto
    {
        public Difficulties AverageDifficulty { get; set; }
        public int TotalRatings { get; set; }
    }

    public class LikeLevelDto
    {
        public int LevelId { get; set; }
    }

    public class LevelLikeResponseDto
    {
        public int TotalLikes { get; set; }
    }
}