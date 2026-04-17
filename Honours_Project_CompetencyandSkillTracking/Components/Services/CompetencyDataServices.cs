using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class CompetencyDataServices
    {
        private readonly AppDbContext _db;

        public CompetencyDataServices(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CompetencyData>> GetCompetencyDataAsync()
        {
            return await _db.CompetencyData
                .Include(c => c.Levels).ThenInclude(l => l.Modules).ThenInclude(clm => clm.Module).ToListAsync();
        }

        public async Task<CompetencyData> AddCompetencyDataAsync(CompetencyData competencyData)
        {
            _db.CompetencyData.Add(competencyData);
            await _db.SaveChangesAsync();
            return competencyData;
        }

        public async Task<CompetencyData?> GetCompetencyByIdAsync(string code)
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).ThenInclude(clm => clm.Module).FirstOrDefaultAsync(c => c.CompetencyCode == code);
        }

        public async Task<List<CompetencyData>> GetCompetenciesByModuleAsync(string modCode)
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).ThenInclude(clm => clm.Module).Where(c => c.Levels.Any(l => l.Modules.Any(clm => clm.Module.ModCode == modCode))).ToListAsync();
        }

        public async Task<List<CompetencyData>> GetAllCompetenciesAsync()
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).ThenInclude(clm => clm.Module).ToListAsync();
        }

        public async Task<CompetencyData> UpdateCompetencyDataAsync(CompetencyData competencyData)
        {
            var existing = await _db.CompetencyData.FirstOrDefaultAsync(c => c.CompetencyDbID == competencyData.CompetencyDbID);

            if (existing == null)
                return competencyData;

            existing.CompetencyName = competencyData.CompetencyName;
            existing.CompetencyCode = competencyData.CompetencyCode;
            existing.AdditionalNotes = competencyData.AdditionalNotes;

            await _db.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteCompetencyDataAsync(CompetencyData competencyData)
        {
            _db.CompetencyData.Remove(competencyData);
            await _db.SaveChangesAsync();
        }
    }
}