using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasOutcomeLinked
    {
        [JsonPropertyName("outcomes")]
        public List<CanvasOutcome> Outcomes { get; set; } = new();
    }
}