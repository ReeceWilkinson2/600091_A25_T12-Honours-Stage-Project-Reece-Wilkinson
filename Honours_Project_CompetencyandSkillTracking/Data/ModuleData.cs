using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class ModuleData
    {
        [Key]
        public int DatabaseID { get; set; } // Primary Key - never seen
        public string Course { get; set; } = "";
        public string Programme { get; set; } = "";
        public string ModuleName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Route { get; set; } = "";
        public string Award { get; set; } = "";
        public string CBOYear { get; set; } = "";
        public string Trimester { get; set; } = "";
        public string SelectionStatus { get; set; } = "";
        public string Level { get; set; } = "";
        public string Credits { get; set; } = "";
        public string ModCode { get; set; } = "";
        public string? VideoLink { get; set; } = "";
    }
}