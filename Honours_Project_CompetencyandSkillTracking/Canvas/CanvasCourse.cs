namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasCourse
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Course_Code { get; set; } = "";
        public string? Syllabus_Body { get; set; }
        public CanvasTerm? Term { get; set; }
    }
}
