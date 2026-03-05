using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasSubmissionComment
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; } = "";

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("author_name")]
        public string? AuthorName { get; set; }
    }
}
