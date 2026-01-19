namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasAssignment
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateTimeOffset? Due_At { get; set; }
    }
}
