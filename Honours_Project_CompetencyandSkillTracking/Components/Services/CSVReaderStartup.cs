using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Honours_Project_CompetencyandSkillTracking.Data;

public class CSVReaderStartup
{
    private readonly CSVReading _CSVReader;
    private readonly AppDbContext _dbContext;

    public CSVReaderStartup(CSVReading csvReader, AppDbContext dbContext)
    {
        _CSVReader = csvReader;
        _dbContext = dbContext;
    }

    public async Task SeedAllAsync()
    {
        var modules = _CSVReader.ReadModules();

        _dbContext.ModulesCSV.RemoveRange(_dbContext.ModulesCSV);
        await _dbContext.SaveChangesAsync();
        await _dbContext.ModulesCSV.AddRangeAsync(modules);
        await _dbContext.SaveChangesAsync();

        var competencies = _CSVReader.ReadCompetencies();

        _dbContext.CompetencyData.RemoveRange(_dbContext.CompetencyData);
        await _dbContext.SaveChangesAsync();
        await _dbContext.CompetencyData.AddRangeAsync(competencies);
        await _dbContext.SaveChangesAsync();

        var competencyLevels = _CSVReader.ReadCompetencyLevels(competencies);

        _dbContext.CompetencyLevels.RemoveRange(_dbContext.CompetencyLevels);
        await _dbContext.SaveChangesAsync();
        await _dbContext.CompetencyLevels.AddRangeAsync(competencyLevels);
        await _dbContext.SaveChangesAsync();
    }
}