// LevelRatingDto.cs
namespace LogizerServer.Models.DTOs
{
    public class RateLevelDto
    {
        public int UserId { get; set; }
        public int LevelId { get; set; }
        public Difficulties DifficultyRating { get; set; }
    }

    public class LevelRatingResponseDto
    {
        public int RatingId { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public Difficulties DifficultyRating { get; set; }
        public DateTime RatedAt { get; set; }
        public Difficulties AverageDifficulty { get; set; }
        public int TotalRatings { get; set; }
    }

    public class LikeLevelDto
    {
        public int UserId { get; set; }
        public int LevelId { get; set; }
    }

    public class LevelLikeResponseDto
    {
        public int LikeId { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public bool IsLiked { get; set; }
        public int TotalLikes { get; set; }
        public DateTime LikedAt { get; set; }
    }
}