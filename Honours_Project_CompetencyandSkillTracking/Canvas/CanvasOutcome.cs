namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasOutcome
    {
        public long Id { get; set; } // Canvas Outcome Id
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CalculationMethod { get; set; }
        public double? MasteryPoints { get; set; }

        public long CourseId { get; set; }
        public Course Course { get; set; }

        public List<CanvasOutcomeResult> OutcomeResults { get; set; } = new();
    }
}
