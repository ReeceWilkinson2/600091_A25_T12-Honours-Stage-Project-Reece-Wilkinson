namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class Submission
    {
        public long Id { get; set; }
        public long AssignmentId { get; set; }
        public double? Score { get; set; }
        //public string WorkflowState { get; set; } = "";
        public DateTime? SubmittedAt { get; set; }
        //public string Comments { get; set; } = "";
    }
}
