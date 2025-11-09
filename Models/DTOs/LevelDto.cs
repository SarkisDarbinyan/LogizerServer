namespace LogizerServer.Models.DTOs
{

    public class CreateLevelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;
    }

    public class GetLevelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;
        public Difficulties Difficulty { get; set; } = Difficulties.Unknown;
        public int LikeCount { get; set; } = 0;
        public int PlayCount { get; set; } = 0;
    }
}