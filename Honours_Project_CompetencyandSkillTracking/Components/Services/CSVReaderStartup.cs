using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class CSVReaderStartup
    {
        private readonly ModuleCSVReading _CSVReader;
        private readonly AppDbContext _dbContext;

        public CSVReaderStartup(ModuleCSVReading csvReader, AppDbContext dbContext)
        {
            _CSVReader = csvReader;
            _dbContext = dbContext;
        }

        public async Task ReadModuleCSV()
        {
            var modules = _CSVReader.ReadModules();

            _dbContext.ModulesCSV.RemoveRange(_dbContext.ModulesCSV);
            await _dbContext.SaveChangesAsync();

            await _dbContext.ModulesCSV.AddRangeAsync(modules);
            await _dbContext.SaveChangesAsync();
        }
    }
}