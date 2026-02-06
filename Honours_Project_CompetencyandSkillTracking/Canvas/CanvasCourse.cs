using System.Text.Json.Serialization;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasCourse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("course_code")]
        public string Course_Code { get; set; } = "";

        [JsonPropertyName("syllabus_body")]
        public string? Syllabus_Body { get; set; }

        [JsonPropertyName("term")]
        public CanvasTerm? Term { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object>? ExtraFields { get; set; }
    }

}
