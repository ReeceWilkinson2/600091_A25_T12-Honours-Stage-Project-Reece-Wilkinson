using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasOutcomeResultsResponse
    {
        [JsonPropertyName("outcome_results")]
        public List<CanvasOutcomeResult> OutcomeResults { get; set; } = new();

        [JsonPropertyName("linked")]
        public CanvasOutcomeLinked Linked { get; set; }
    }
}