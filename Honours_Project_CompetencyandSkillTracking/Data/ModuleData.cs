using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class ModuleData
    {
        [Key]
        public int DatabaseID { get; set; }
        public string Course { get; set; } = ""; // Treat it like an ID
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
    }
}