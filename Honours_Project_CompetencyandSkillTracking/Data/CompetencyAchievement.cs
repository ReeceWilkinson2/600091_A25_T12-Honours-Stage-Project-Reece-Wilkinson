using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyAchievement
    {
        [Key]
        public int Id { get; set; }
        public string StudentId { get; set; } = "";
        public User User { get; set; } = null!;
        public int CompetencyLevelId { get; set; }
        public CompetencyLevels CompetencyLevel { get; set; } = null!;
        public int MasteryPoints { get; set; }
        public DateTime AchievedDate { get; set; }
    }
}
