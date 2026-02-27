using System.Text.Json.Serialization;
using System;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.DTOs
{
    public class CanvasSubmission
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("assignment_id")]
        public long AssignmentId { get; set; }

        [JsonPropertyName("score")]
        public double? Score { get; set; }

        //[JsonPropertyName("workflow_state")]
        //public string WorkflowState { get; set; } = string.Empty;

        [JsonPropertyName("submitted_at")]
        public DateTime? SubmittedAt { get; set; }

        //[JsonPropertyName("comments")]
        //public string Comments { get; set; } = string.Empty;

        [JsonExtensionData]
        public Dictionary<string, object>? ExtraFields { get; set; }

    }
}
