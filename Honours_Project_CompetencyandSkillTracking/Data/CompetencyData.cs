using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyData
    {
        [Key]
        public int CompetencyDbID { get; set; } // Should auto increment
        public string CompetencyName { get; set; } = "";
        public string CompetencyID { get; set; } = "";
        public string? AdditionalNotes { get; set; }
        public ICollection<CompetencyLevels> Levels { get; set; } = new List<CompetencyLevels>();
    }
}