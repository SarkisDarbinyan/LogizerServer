// User.cs
namespace LogizerServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public ICollection<UserFavorite> FavoriteLevels { get; set; } = new List<UserFavorite>();
        public ICollection<LevelRating> LevelRatings { get; set; } = new List<LevelRating>();
        public ICollection<LevelLike> LevelLikes { get; set; } = new List<LevelLike>();
    }
}