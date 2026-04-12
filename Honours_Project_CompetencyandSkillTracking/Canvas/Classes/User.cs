using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Canvas.Classes
{
    //[Index(nameof(StudentId), IsUnique = true)]
        public class User
    {
        [Key]
        public string StudentId { get; set; } // Canvas user id, primary key
        public string UserName { get; set; } = "";
        public string StEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

        // Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<CompetencyAchievement> CompetencyAchievements { get; set; }= new List<CompetencyAchievement>();
    }
}