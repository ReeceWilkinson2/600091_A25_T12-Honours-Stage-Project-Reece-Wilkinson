using Honours_Project_CompetencyandSkillTracking.Canvas.DTOs;
using System.Text.Json.Serialization;

public class CanvasOutcomeResult
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("score")]
    public double? Score { get; set; }

    [JsonPropertyName("mastery")]
    public bool? Mastery { get; set; }

    // Nested outcome object
    [JsonPropertyName("outcome")]
    public CanvasOutcome Outcome { get; set; }

    // Nested links object
    [JsonPropertyName("links")]
    public CanvasOutcomeResultLinks Links { get; set; }

    public string StudentId { get; set; }
}