using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; } // Canvas user id, primary key
        public string UserName { get; set; } = "";
        public string StEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

    }
}
