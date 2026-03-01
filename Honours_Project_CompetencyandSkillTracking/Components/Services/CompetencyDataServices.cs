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
        #region Private members
        private AppDbContext _db;
        #endregion

        #region Constructor
        public CompetencyDataServices(AppDbContext db)
        {
            _db = db;
        }
        #endregion

        #region Public methods
        public async Task<List<CompetencyData>> GetCompetencyDataAsync()
        {
            return await _db.CompetencyData
                //.Where(c => c.Course == CourseId)
                .ToListAsync();
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

        public async Task<CompetencyData> GetCompetencyByIdAsync(string id)
        {
            return await _db.CompetencyData
                .FirstOrDefaultAsync(c => c.CompetencyID == id);
        }

        public async Task<CompetencyData> UpdateCompetencyDataAsync(CompetencyData CompetencyData)
        {
            try
            {
                var CompetencyDataExist = _db.CompetencyData.FirstOrDefault(p => p.CompetencyDbID == CompetencyData.CompetencyDbID);
                if (CompetencyDataExist != null)
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
        #endregion
    }
}