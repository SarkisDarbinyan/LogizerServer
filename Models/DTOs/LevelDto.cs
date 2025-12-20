namespace LogizerServer.Models.DTOs
{
    public class CreateLevelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;
        public int UserId { get; set; } // Добавлено: ID пользователя, создающего уровень
    }

    public class GetLevelDto
    {
        public int LevelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;
        public Difficulties Difficulty { get; set; } = Difficulties.Unknown;
        public int LikeCount { get; set; } = 0;
        public int PlayCount { get; set; } = 0;
        public int CreatorId { get; set; } // Добавлено: ID создателя уровня
    }
}