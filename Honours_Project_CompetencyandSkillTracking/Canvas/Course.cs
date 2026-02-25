using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class Course
    {
        [Key]
        public long Id { get; set; }       // Canvas course id
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string? Syllabus { get; set; }
        public string Term { get; set; } = "";

        // Navigation
        public ICollection<User> Students { get; set; } = new List<User>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
