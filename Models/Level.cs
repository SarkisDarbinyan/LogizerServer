// Level.cs
namespace LogizerServer.Models
{
    public class Level
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;

        public int CreatorId { get; set; } // Добавлено: ID пользователя-создателя
        public Difficulties Difficulty { get; set; } = Difficulties.Unknown;
        public int LikeCount { get; set; } = 0;
        public int PlayCount { get; set; } = 0;

        // Навигационные свойства
        public User? Creator { get; set; } // Добавлено: навигационное свойство
        public ICollection<LevelRating> Ratings { get; set; } = new List<LevelRating>();
        public ICollection<LevelLike> Likes { get; set; } = new List<LevelLike>();
    }

    public enum Difficulties { Unknown, Easy, Normal, Hard, Insane }
}