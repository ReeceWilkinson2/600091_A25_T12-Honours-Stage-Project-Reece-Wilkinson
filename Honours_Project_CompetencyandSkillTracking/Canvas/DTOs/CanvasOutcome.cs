namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    using System.Text.Json.Serialization;

    public class CanvasOutcome
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("calculation_method")]
        public string CalculationMethod { get; set; }

        [JsonPropertyName("mastery_points")]
        public double? MasteryPoints { get; set; }
    }

}
