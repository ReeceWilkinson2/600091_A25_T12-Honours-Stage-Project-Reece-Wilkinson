using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyLevels
    {
        [Key]
        public int LevelDbID { get; set; }
        public int LevelNumber { get; set; }
        public string Description { get; set; }
        public int CompetencyDbID { get; set; }
        public CompetencyData Competency { get; set; }
    }
}