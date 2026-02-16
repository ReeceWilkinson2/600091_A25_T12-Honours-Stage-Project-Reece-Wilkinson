namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasOutcomeResult
    {
        public long Id { get; set; } // Canvas result id
        public long OutcomeId { get; set; }
        public CanvasOutcome Outcome { get; set; }

        public long? AssignmentId { get; set; } // Optional link to assignment
        public string StudentId { get; set; }
        public User Student { get; set; }

        public double? Score { get; set; }
        public bool? Mastery { get; set; }

        // Optional computed mastery
        public bool ComputedMastery => Outcome != null && Score >= Outcome.MasteryPoints;
    }
}
