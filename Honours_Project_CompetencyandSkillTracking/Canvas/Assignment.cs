namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class Assignment
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateTimeOffset? DueAt { get; set; }
    }
}
