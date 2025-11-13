// UserFavoriteDto.cs
namespace LogizerServer.Models.DTOs
{
    public class AddFavoriteDto
    {
        public int UserId { get; set; }
        public int LevelId { get; set; }
    }

    public class FavoriteLevelDto
    {
        public int FavoriteId { get; set; }
        public int LevelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Difficulties Difficulty { get; set; }
        public int LikeCount { get; set; }
        public int PlayCount { get; set; }
        public DateTime AddedAt { get; set; }
    }
}