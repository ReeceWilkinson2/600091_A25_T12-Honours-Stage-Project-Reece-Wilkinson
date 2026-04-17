namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyLevelModule
    {
        // Join table
        public int CompetencyLevelId { get; set; }
        public CompetencyLevels CompetencyLevel { get; set; } = null!;

        public int ModuleId { get; set; }
        public ModuleData Module { get; set; } = null!;
    }
}