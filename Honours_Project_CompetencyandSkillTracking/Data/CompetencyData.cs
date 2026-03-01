using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyData
    {
        [Key]
        public int CompetencyDbID { get; set; } // Should auto increment
        public string CompetencyName { get; set; }
        public string CompetencyID { get; set; }
        public string Module { get; set; }
        public string Course { get; set; }
        public string Level4Description { get; set; }
        public string Level5Description { get; set; }
        public string Level6Description { get; set; }
    }
}
