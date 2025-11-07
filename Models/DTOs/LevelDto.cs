namespace LogizerServer.Models.DTOs
{

    public class CreateLevelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;
    }

    public class UpdateLevelDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelData { get; set; } = string.Empty;
    }
}