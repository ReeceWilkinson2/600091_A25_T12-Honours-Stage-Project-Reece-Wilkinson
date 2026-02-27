namespace Honours_Project_CompetencyandSkillTracking.Canvas.Classes
{
    public class Assignment
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateTimeOffset? DueAt { get; set; }

        // Navigation
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<Outcome> Outcomes { get; set; } = new List<Outcome>();
    }
}
