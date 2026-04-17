using Honours_Project_CompetencyandSkillTracking.Data;
using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyLevels
    {
        [Key]
        public int LevelDbID { get; set; }
        public int LevelNumber { get; set; }
        public string Description { get; set; } = "";
        public int CompetencyDbID { get; set; }

        // get rid of this when the csvs are done
        public string ModCode { get; set; } = "";
        public CompetencyData Competency { get; set; } = null!;
        // Navigation
        public ICollection<CompetencyLevelModule> Modules { get; set; } = new List<CompetencyLevelModule>(); // comp levels can have multiple modules associated with them
        public ICollection<CompetencyAchievement> Achievements { get; set; } = new List<CompetencyAchievement>();
    }
}