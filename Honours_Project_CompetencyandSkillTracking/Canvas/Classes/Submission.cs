namespace Honours_Project_CompetencyandSkillTracking.Canvas.Classes
{
    public class Submission
    {
        public long Id { get; set; }
        public long AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
        public string StudentId { get; set; } = "";
        public User Student { get; set; } = null!;
        public double? Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public ICollection<SubmissionComment> Comments { get; set; } = new List<SubmissionComment>();
        public ICollection<OutcomeResult> OutcomeResults { get; set; } = new List<OutcomeResult>();
    }
}
