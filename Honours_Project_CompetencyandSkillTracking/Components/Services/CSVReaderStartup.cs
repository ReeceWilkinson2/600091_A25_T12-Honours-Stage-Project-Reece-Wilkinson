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
        var competencies = _CSVReader.ReadCompetencies();
        var competencyLevels = _CSVReader.ReadCompetencyLevels(competencies);

        // Clear previous data
        _dbContext.ModulesCSV.RemoveRange(_dbContext.ModulesCSV);
        _dbContext.CompetencyData.RemoveRange(_dbContext.CompetencyData);
        _dbContext.CompetencyLevels.RemoveRange(_dbContext.CompetencyLevels);
        await _dbContext.SaveChangesAsync();

        await _dbContext.ModulesCSV.AddRangeAsync(modules);
        await _dbContext.CompetencyData.AddRangeAsync(competencies);
        await _dbContext.CompetencyLevels.AddRangeAsync(competencyLevels);
        await _dbContext.SaveChangesAsync();

        var users = _CSVReader.ReadUsers();
        //_dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.SaveChangesAsync();
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();

        var courses = _CSVReader.ReadCourses(users);
        _dbContext.Courses.RemoveRange(_dbContext.Courses);
        await _dbContext.SaveChangesAsync();
        await _dbContext.Courses.AddRangeAsync(courses);
        await _dbContext.SaveChangesAsync();
    }
}