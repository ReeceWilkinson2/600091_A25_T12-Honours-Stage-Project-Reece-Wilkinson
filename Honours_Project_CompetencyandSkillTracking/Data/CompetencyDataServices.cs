using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyDataServices
    {
        #region Private members
        private CompetencyInfoDbContext dbContext;
        #endregion

        #region Constructor
        public CompetencyDataServices(CompetencyInfoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #endregion

        #region Public methods
        public async Task<List<CompetencyData>> GetCompetencyDataAsync()
        {
            return await dbContext.CompetencyData.ToListAsync();
        }

        public async Task<CompetencyData> AddCompetencyDataAsync(CompetencyData CompetencyData)
        {
            try
            {
                dbContext.CompetencyData.Add(CompetencyData);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return CompetencyData;
        }

        public async Task<CompetencyData> UpdateCompetencyDataAsync(CompetencyData CompetencyData)
        {
            try
            {
                var CompetencyDataExist = dbContext.CompetencyData.FirstOrDefault(p => p.CompetencyDbID == CompetencyData.CompetencyDbID);
                if (CompetencyDataExist != null)
                {
                    dbContext.Update(CompetencyData);
                    await dbContext.SaveChangesAsync();
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
                dbContext.CompetencyData.Remove(CompetencyData);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
