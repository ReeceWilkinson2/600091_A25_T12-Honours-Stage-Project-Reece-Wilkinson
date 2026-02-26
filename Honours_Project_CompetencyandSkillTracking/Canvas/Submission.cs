namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class Submission
    {
        public long Id { get; set; }
        public long AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public string StudentId { get; set; }
        public User Student { get; set; }
        public double? Score { get; set; }
        //public string WorkflowState { get; set; } = "";
        public DateTime? SubmittedAt { get; set; }
        //public string Comments { get; set; } = "";
        public ICollection<OutcomeResult> OutcomeResults { get; set; } = new List<OutcomeResult>();
    }
}
