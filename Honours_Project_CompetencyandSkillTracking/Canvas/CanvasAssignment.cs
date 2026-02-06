using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasAssignment
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("due_at")]
        public DateTimeOffset? Due_At { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object>? ExtraFields { get; set; }
    }
}
