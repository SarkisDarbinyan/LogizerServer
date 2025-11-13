// UserFavorite.cs
namespace LogizerServer.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LevelId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public User User { get; set; } = null!;
        public Level Level { get; set; } = null!;
    }
}