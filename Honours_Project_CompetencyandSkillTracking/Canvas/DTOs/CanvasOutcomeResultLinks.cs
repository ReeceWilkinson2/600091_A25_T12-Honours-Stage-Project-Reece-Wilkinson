using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasOutcomeResultLinks
    {
        [JsonPropertyName("assignment")]
        public string? Assignment { get; set; }
    }
}