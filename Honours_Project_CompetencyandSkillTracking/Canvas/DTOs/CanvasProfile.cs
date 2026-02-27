using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasProfile
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("primary_email")]
        public string Primary_Email { get; set; } = "";

        [JsonExtensionData]
        public Dictionary<string, object>? ExtraFields { get; set; }
    }
}
