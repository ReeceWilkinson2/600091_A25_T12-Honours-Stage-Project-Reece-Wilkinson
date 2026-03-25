using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class CompetencyDataServices
    {
        private AppDbContext _db;

        public CompetencyDataServices(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CompetencyData>> GetCompetencyDataAsync()
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).ToListAsync();
        }

        public async Task<CompetencyData> AddCompetencyDataAsync(CompetencyData CompetencyData)
        {
            try
            {
                _db.CompetencyData.Add(CompetencyData);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return CompetencyData;
        }

        public async Task<CompetencyData?> GetCompetencyByIdAsync(string id)
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).FirstOrDefaultAsync(c => c.CompetencyID == id);
        }

        public async Task<List<CompetencyData>> GetCompetenciesByModuleAsync(string modCode)
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).Where(c => c.Levels.Any(l => l.Modules.Any(m => m.ModCode == modCode))).ToListAsync();
        }

        public async Task<List<CompetencyData>> GetAllCompetenciesAsync()
        {
            return await _db.CompetencyData.Include(c => c.Levels).ThenInclude(l => l.Modules).ToListAsync();
        }

        public async Task<CompetencyData> UpdateCompetencyDataAsync(CompetencyData CompetencyData)
        {
            try
            {
                var competencyDataExist = _db.CompetencyData.Include(c => c.Levels).FirstOrDefault(p => p.CompetencyDbID == CompetencyData.CompetencyDbID);
                if (competencyDataExist != null)
                {
                    _db.Update(CompetencyData);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return CompetencyData;
        }

        public async Task DeleteCompetencyDataAsync(CompetencyData CompetencyData)
        {
            try
            {
                _db.CompetencyData.Remove(CompetencyData);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}