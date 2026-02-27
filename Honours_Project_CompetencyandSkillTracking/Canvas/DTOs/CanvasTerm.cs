using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasTerm
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonExtensionData]
        public Dictionary<string, object>? ExtraFields { get; set; }
    }
}
