using Honours_Project_CompetencyandSkillTracking.Data;
using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class User
    {
        [Key]
        public string StudentId { get; set; } // Canvas user id, primary key
        public string UserName { get; set; } = "";
        public string StEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

        //// Navigation properties (if they exist)
        //public List<Competency> Competencies { get; set; } = new List<Competency>(); // Assuming a User has many Competencies
        //public List<Skill> Skills { get; set; } = new List<Skill>();  // Assuming a User has many Skills
        //public List<ModuleData> Modules { get; set; } = new List<ModuleData>(); // Assuming a User has many Modules
    }
}
