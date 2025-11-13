// LevelRating.cs
namespace LogizerServer.Models
{
    public class LevelRating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LevelId { get; set; }
        public Difficulties DifficultyRating { get; set; }
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public User User { get; set; } = null!;
        public Level Level { get; set; } = null!;
    }
}

// LevelLike.cs
namespace LogizerServer.Models
{
    public class LevelLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LevelId { get; set; }
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public User User { get; set; } = null!;
        public Level Level { get; set; } = null!;
    }
}