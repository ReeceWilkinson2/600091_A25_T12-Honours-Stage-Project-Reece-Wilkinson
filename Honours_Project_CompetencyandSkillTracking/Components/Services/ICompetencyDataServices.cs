using Honours_Project_CompetencyandSkillTracking.Data;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public interface ICompetencyDataServices
    {
        Task<List<CompetencyData>> GetCompetencyDataAsync();
        Task<CompetencyData> AddCompetencyDataAsync(CompetencyData competencyData);
        Task<CompetencyData?> GetCompetencyByIdAsync(string code);
        Task<List<CompetencyData>> GetCompetenciesByModuleAsync(string modCode);
        Task<List<CompetencyData>> GetAllCompetenciesAsync();
        Task<CompetencyData> UpdateCompetencyDataAsync(CompetencyData competencyData);
        Task DeleteCompetencyDataAsync(CompetencyData competencyData);
    }
}