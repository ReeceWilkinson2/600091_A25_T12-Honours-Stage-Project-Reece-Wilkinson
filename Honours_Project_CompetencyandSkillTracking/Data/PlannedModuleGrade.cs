namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class PlannedModuleGrade
    {
        public string ModCode { get; set; } = "";
        public Dictionary<long, double> AssignmentScores { get; set; } = new();
    }
}
