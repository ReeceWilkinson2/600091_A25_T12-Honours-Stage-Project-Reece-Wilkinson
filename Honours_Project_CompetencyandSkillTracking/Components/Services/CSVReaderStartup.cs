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
        // Need to save all tables separately so the data can be referenced.
        var modules = _CSVReader.ReadModules();
        _dbContext.ModulesCSV.RemoveRange(_dbContext.ModulesCSV);
        await _dbContext.ModulesCSV.AddRangeAsync(modules);
        await _dbContext.SaveChangesAsync();

        var competencies = _CSVReader.ReadCompetencies();
        _dbContext.CompetencyData.RemoveRange(_dbContext.CompetencyData);
        await _dbContext.CompetencyData.AddRangeAsync(competencies);

        var competencyLevels = _CSVReader.ReadCompetencyLevels(competencies);
        _dbContext.CompetencyLevels.RemoveRange(_dbContext.CompetencyLevels);
        await _dbContext.CompetencyLevels.AddRangeAsync(competencyLevels);

        var users = _CSVReader.ReadUsers();
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.Users.AddRangeAsync(users);

        var courses = _CSVReader.ReadCourses(users);
        _dbContext.Courses.RemoveRange(_dbContext.Courses);
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