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
        _dbContext.ModulesCSV.RemoveRange(_dbContext.ModulesCSV);
        await _dbContext.ModulesCSV.AddRangeAsync(modules);
        await _dbContext.SaveChangesAsync();

        var competencies = _CSVReader.ReadCompetencies();
        _dbContext.CompetencyData.RemoveRange(_dbContext.CompetencyData);
        await _dbContext.CompetencyData.AddRangeAsync(competencies);
        await _dbContext.SaveChangesAsync();

        var competencyLevels = _CSVReader.ReadCompetencyLevels(competencies);
        _dbContext.CompetencyLevels.RemoveRange(_dbContext.CompetencyLevels);
        await _dbContext.CompetencyLevels.AddRangeAsync(competencyLevels);
        await _dbContext.SaveChangesAsync();

        var competencyLevelModules = _CSVReader.ReadCompetencyLevelModules(competencyLevels, modules);
        _dbContext.CompetencyLevelModules.RemoveRange(_dbContext.CompetencyLevelModules);
        await _dbContext.CompetencyLevelModules.AddRangeAsync(competencyLevelModules);
        await _dbContext.SaveChangesAsync();

        var users = _CSVReader.ReadUsers();
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();

        var courses = _CSVReader.ReadCourses(users);
        _dbContext.Courses.RemoveRange(_dbContext.Courses);
        await _dbContext.Courses.AddRangeAsync(courses);
        await _dbContext.SaveChangesAsync();

        var assignments = _CSVReader.ReadAssignments(courses);
        _dbContext.Assignments.RemoveRange(_dbContext.Assignments);
        await _dbContext.Assignments.AddRangeAsync(assignments);
        await _dbContext.SaveChangesAsync();

        var submissions = _CSVReader.ReadSubmissions(assignments, users);
        _dbContext.Submissions.RemoveRange(_dbContext.Submissions);
        await _dbContext.Submissions.AddRangeAsync(submissions);
        await _dbContext.SaveChangesAsync();

        var comments = _CSVReader.ReadSubmissionComments(submissions);
        _dbContext.SubmissionComments.RemoveRange(_dbContext.SubmissionComments);
        await _dbContext.SubmissionComments.AddRangeAsync(comments);
        await _dbContext.SaveChangesAsync();

        var achievements = _CSVReader.ReadCompetencyAchievements(users, competencyLevels, modules, submissions);
        _dbContext.CompetencyAchievements.RemoveRange(_dbContext.CompetencyAchievements);
        await _dbContext.CompetencyAchievements.AddRangeAsync(achievements);
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