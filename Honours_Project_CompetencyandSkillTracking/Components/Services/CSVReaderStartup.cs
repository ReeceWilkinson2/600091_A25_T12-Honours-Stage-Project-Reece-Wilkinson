using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;

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
        var users = _CSVReader.ReadUsers();
        var courses = _CSVReader.ReadCourses(users);

        // Clear previous data
        _dbContext.ModulesCSV.RemoveRange(_dbContext.ModulesCSV);
        _dbContext.CompetencyData.RemoveRange(_dbContext.CompetencyData);
        _dbContext.CompetencyLevels.RemoveRange(_dbContext.CompetencyLevels);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Courses.RemoveRange(_dbContext.Courses);
        await _dbContext.SaveChangesAsync();

        await _dbContext.ModulesCSV.AddRangeAsync(modules);
        await _dbContext.CompetencyData.AddRangeAsync(competencies);
        await _dbContext.CompetencyLevels.AddRangeAsync(competencyLevels);
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.Courses.AddRangeAsync(courses);
        await _dbContext.SaveChangesAsync();

        var admin = new User
        {
            StudentId = "admin-001",
            UserName = "wewo",
            StEmail = "test@email",
            Password = "test",
            Role = "Admin"
        };

        if (!_dbContext.Users.Any(u => u.StudentId == admin.StudentId))
        {
            await _dbContext.Users.AddAsync(admin);
            await _dbContext.SaveChangesAsync();
        }
    }
}