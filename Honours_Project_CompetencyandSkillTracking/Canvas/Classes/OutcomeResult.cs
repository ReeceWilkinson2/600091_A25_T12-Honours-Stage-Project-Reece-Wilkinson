namespace Honours_Project_CompetencyandSkillTracking.Canvas.Classes
{
    public class OutcomeResult
    {
        public long Id { get; set; } // Canvas result id
        public long OutcomeId { get; set; }
        public Outcome Outcome { get; set; } = null!;

        public long? AssignmentId { get; set; } // Optional link to assignment
        public string StudentId { get; set; } = "";
        public User Student { get; set; } = null!;

        public double? Score { get; set; }
        public bool? Mastery { get; set; }

        // Optional computed mastery
        public bool ComputedMastery => Outcome != null && Score.HasValue && Outcome.MasteryPoints.HasValue && Score.Value >= Outcome.MasteryPoints.Value;

        public long? SubmissionId { get; set; }
        public Submission? Submission { get; set; }
    }
}