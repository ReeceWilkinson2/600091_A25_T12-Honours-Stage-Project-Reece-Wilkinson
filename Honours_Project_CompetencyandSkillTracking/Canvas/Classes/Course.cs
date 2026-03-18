using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.Classes
{
    public class Course
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string? Syllabus { get; set; }
        public string Term { get; set; } = "";
        public string StudentId { get; set; }
        public User Student { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Outcome> Outcomes { get; set; } = new List<Outcome>();
    }
}