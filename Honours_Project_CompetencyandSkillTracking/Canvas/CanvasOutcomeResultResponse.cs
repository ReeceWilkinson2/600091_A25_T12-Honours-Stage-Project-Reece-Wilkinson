using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasOutcomeResultsResponse
    {
        [JsonPropertyName("outcome_results")]
        public List<OutcomeResult> OutcomeResults { get; set; } = new();
    }
}
