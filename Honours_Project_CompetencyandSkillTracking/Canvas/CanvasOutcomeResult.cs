using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasOutcomeResult
    {
        public long Id { get; set; }
        public double? Score { get; set; }
        public bool? Mastery { get; set; }
        public string StudentId { get; set; }
        public CanvasOutcome Outcome { get; set; }

        // Optional link to assignment, comes from Canvas API JSON "links"
        [JsonPropertyName("links")]
        public CanvasOutcomeResultLinks Links { get; set; }
    }
}
