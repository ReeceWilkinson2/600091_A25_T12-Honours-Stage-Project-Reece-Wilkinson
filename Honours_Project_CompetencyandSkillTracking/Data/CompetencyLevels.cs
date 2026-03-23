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
        public string ModCode { get; set; } = "";
        public CompetencyData Competency { get; set; }
        // experiment with required fields for ths
        // Navigation
        public ICollection<CompetencyAchievement> Achievements { get; set; } = new List<CompetencyAchievement>();
    }
}

//    public ICollection<ModuleData> Modules { get; set; } = new List<ModuleData>();